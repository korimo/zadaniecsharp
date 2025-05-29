import os
from openai import OpenAI
from dotenv import load_dotenv

# načítanie .env
load_dotenv()

api_key = os.getenv("OPENAI_API_KEY")
if not api_key:
    print("❌ OPENAI_API_KEY nie je nastavený v .env súbore.")
    exit(1)

client = OpenAI(api_key=api_key)

try:
    response = client.chat.completions.create(
        model="gpt-4",
        messages=[
            {"role": "user", "content": "Napíš krátky text o výrobe nábytku"}
        ],
        temperature=0.5,
    )
    print("✅ API odpoveď:\n")
    print(response.choices[0].message.content)

except Exception as e:
    print(f"❌ Chyba: {e}")
