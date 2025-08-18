import requests
import json
import tkinter as tk
from tkinter import messagebox, scrolledtext
import ipaddress

def get_api_key():
    while True:
        try:
            key = input("Entrez votre clé API : ").strip()
            if not key or len(key) < 30:
                raise ValueError("Clé API invalide")
            return key
        except ValueError as e:
            print(f"Erreur : {e}")

API_KEY = get_api_key()
API_URL = "https://api.abuseipdb.com/api/v2/check"

def check_ip(ip_address):
    try:
        ipaddress.ip_address(ip_address)
    except ValueError:
        messagebox.showerror("Erreur", "Adresse IP invalide.")
        return

    headers = {
        "Key": API_KEY,
        "Accept": "application/json"
    }
    params = {
        "ipAddress": ip_address,
        "maxAgeInDays": 90
    }

    response = requests.get(API_URL, headers=headers, params=params)

    if response.status_code == 200:
        data = response.json()["data"]
        abuse_score = data.get("abuseConfidenceScore", 0)

        if abuse_score >= 50:
            status = "Suspect"
        elif abuse_score >= 20:
            status = "À surveiller"
        else:
            status = "Propre"

        result = f"""
 Résultats pour : {ip_address}
 Pays : {data.get("countryName", "N/A")}
 Fournisseur : {data.get("isp", "N/A")}
 Nombre de signalements : {data.get("totalReports", "N/A")}
 Dernier signalement : {data.get("lastReportedAt", "N/A")}
 Score de réputation : {abuse_score} ({status})
"""
        result_box.config(state="normal")
        result_box.delete(1.0, tk.END)
        result_box.insert(tk.END, result)
        result_box.config(state="disabled")
    else:
        messagebox.showerror("Erreur API", f"Code : {response.status_code}\n{response.text}")

window = tk.Tk()
window.title("IP Reputation Checker")
window.geometry("500x400")
window.resizable(False, False)

tk.Label(window, text="Adresse IP à analyser :", font=("Arial", 12)).pack(pady=10)
ip_entry = tk.Entry(window, font=("Arial", 12), width=30)
ip_entry.pack()

tk.Button(window, text="Analyser", font=("Arial", 12), command=lambda: check_ip(ip_entry.get())).pack(pady=10)

result_box = scrolledtext.ScrolledText(window, font=("Courier", 10), width=60, height=12, state="disabled")
result_box.pack(pady=10)

window.mainloop()
