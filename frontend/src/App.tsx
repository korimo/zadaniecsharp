// src/App.tsx
import React, { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Textarea } from '@/components/ui/textarea';
import { motion } from 'framer-motion';
import axios from 'axios';

const API_BASE = import.meta.env.VITE_API_URL || '';
const App = () => {
  const [prompt, setPrompt] = useState('');
  const [htmlPreview, setHtmlPreview] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [readyToDownload, setReadyToDownload] = useState(false);

  const handleGenerate = async (retries = 3) => {
    setLoading(true);
    setReadyToDownload(false); // ⬅️ RESET na začiatku
    setError('');
    setReadyToDownload(false);
    try {
      const response = await axios.post('${API_BASE}/api/generate', { prompt });
      if (response.data.html) {
        setHtmlPreview(response.data.html);
        setReadyToDownload(true);
      } else {
        throw new Error('Prázdna odpoveć z API');
      }
    } catch (err: any) {
      console.error('Chyba pri generovaní:', err);
      if (retries > 0) {
        console.warn(`⏳ Retry... ${retries}`);
        setTimeout(() => handleGenerate(retries - 1), 1000);
        return;
      }
      setError('Chyba pri generovaní webu. Skontrolujte API alebo sieť.');
    }
    setLoading(false);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-200 p-6">
      <div className="max-w-6xl mx-auto">
        <h1 className="text-4xl font-bold mb-4 text-center">🧠 AI Web Builder</h1>
        <Card>
          <CardContent className="p-4">
            <Textarea
              placeholder="Zadajte popis webu, napr. 'Web pre výrobcu nábytku'..."
              value={prompt}
              onChange={(e) => setPrompt(e.target.value)}
              className="mb-4 min-h-[120px]"
            />
            <Button onClick={() => handleGenerate()} disabled={loading} className="w-full">
              {loading ? 'Generovanie...' : 'Vygeneruj web'}
            </Button>
            {error && (
              <div className="mt-4 text-red-600 text-sm text-center">
                {error}
              </div>
            )}
          </CardContent>
        </Card>

        {htmlPreview && (
          <div className="mt-10 border rounded shadow bg-white">
            <iframe
              srcDoc={htmlPreview}
              className="w-full h-[1000px] border-none rounded"
              title="AI Náhľad"
            />
            {readyToDownload && (
              <div className="text-center mt-6">
                <a
                  href="${API_BASE}/api/download"
                  download
                  className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
                >
                  Stiahnuť ZIP
                </a>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default App;
