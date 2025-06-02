import fetch from "node-fetch";

const URL = "https://zadaniecsharp.onrender.com/";

console.log("Keepalive worker started...");

setInterval(() => {
  fetch(URL)
    .then(res => res.text())
    .then(text => console.log(`[${new Date().toISOString()}] Ping OK: ${text}`))
    .catch(err => console.error("Ping failed:", err.message));
}, 50000);
