# 🧠 AI Web Generator

Jednoduchý AI nástroj na návrh webstránky z textového zadania. Postavené na React (frontend) a ASP.NET Core (backend) s využitím OpenAI GPT-4.

## 🛠️ Inštalácia

### Backend (C# ASP.NET Core)

1. **Nainštaluj závislosti:**
   ```bash
   dotnet restore
   ```

2. **Pridaj API kľúč do `appsettings.json`:**
   ```json
   {
     "OpenAI": {
       "ApiKey": "YOUR_OPENAI_API_KEY"
     }
   }
   ```

3. **Spusti server:**
   ```bash
   dotnet run
   ```

### Frontend (React + Vite)

1. **Nainštaluj závislosti:**
   ```bash
   npm install
   ```

2. **Spusti vývojový server:**
   ```bash
   npm run dev
   ```

3. **Uprav `vite.config.ts` ak treba proxy k backendu** (napr. `localhost:5000`)

---

## 📤 API Endpoint

- **POST** `/api/generate`
- **Body:** `{ "prompt": "Web pre výrobcu nábytku" }`
- **Response:** `{ html: "<html>...</html>" }`

---

## ✅ Funkcie

- 📝 Zadanie popisu webu (textarea)
- 💡 AI vygeneruje HTML + CSS + texty + obrázky (alt text)
- 🎨 Náhodné, ale ladiace farby
- 🔁 Fallback HTML ak AI výstup zlyhá

---

## 📦 Použité technológie
- ASP.NET Core 8 Web API
- React + TailwindCSS + shadcn/ui
- OpenAI GPT-4 API
- Axios, Framer Motion

---

## 📄 Licencia
MIT
