# 🛰️ SvenScope

**SvenScope** is a simple and powerful **terminal-based tracker** for Sven Co-op servers —  
built entirely in **Python**, with an interactive CLI that lets you add, view, and monitor servers easily.

---

## 📖 Table of Contents

1. [Features](#-features)
2. [Tech Stack](#-tech-stack)
3. [Installation](#%EF%B8%8F-installation)
4. [Usage](#-usage)
5. [Example Output](#-example-output)

---

## 🚀 Features

✅ Add new servers (supports both `IP` and `IP:PORT` formats)  
✅ View all saved servers  
✅ Query live server info (name, map, players, etc.)  
✅ Data stored locally in `servers.json`  
✅ Fully interactive menu using `InquirerPy`  
✅ Works on **Windows**, **Linux**, and **macOS**

---

## 🧰 Tech Stack

- **Python 3.10+**
- [`python-a2s`](https://pypi.org/project/python-a2s/) — for querying Sven Co-op servers  
- [`InquirerPy`](https://github.com/kazhala/InquirerPy) — for a clean and interactive terminal interface  

---

## ⚙️ Installation

Clone the repository and set up your environment:

```bash
git clone https://github.com/MR11Robot/SvenScope.git
cd SvenScope
py -m venv .venv
source .venv/bin/activate   # On Windows: .venv\Scripts\activate
pip install -r requirements.txt
```

---

## 🕹️ Usage

Run the tracker:

```bash
py tracker.py
```

Then choose one of the available options:

- **Add new server** → Add by typing either `IP` or `IP:PORT`
- **List saved servers** → Show all added servers
- **Start tracking** → Query and display live status (map, players, etc.)
- **Exit** → Close the app

---

## 💾 Example Output

```
=======================================
        🛰️  Sven Co-op Tracker
=======================================

🟢 Server: 150.20.108.197:1337
🏷️  Name : Sven Co-op Public Server
🗺️  Map  : svencoop1
👥 Players (3/16)
   01. Mr_Robot
   02. SvenBot
   03. Guest123
---------------------------------------

✅ Query completed.
```

