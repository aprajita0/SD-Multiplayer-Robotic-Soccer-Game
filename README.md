# Mechanix Legions – Multiplayer Robotic Soccer Simulation

**Senior Design Project – CSC 59867**  
**City College of New York**  
**Developed by: Aprajita Srivastava & Jezlea Ortega**  
**Advisor: Professor Kaliappa**

---

## 📌 Project Overview

Mechanix Legions is a real-time multiplayer robotic soccer simulation built in Unity. The project focuses on implementing AI-controlled teams and a low-level custom TCP server to simulate synchronized gameplay across multiple agents, following the **WYSIWIS (What You See Is What I See)** paradigm.

Each team consists of five AI-controlled robots assigned with specific roles—attacker, midfielder, defender, and goalkeeper—operating within a coordinated decision-making system. The game emphasizes real-time coordination, team strategy, AI adaptability, and fault tolerance.

---

## 🚀 Features

- Autonomous AI behavior using NavMesh for role-based movement.
- Custom-built multithreaded TCP server using `.NET TcpListener`.
- Real-time client-server communication with positional synchronization.
- Role-based tactical decisions: passing, shooting, positioning.
- Fault injection system to simulate perception and action errors.
- Minimalist HUD with live scoreboard and match timer.
- Physics-based soccer ball interactions and goal detection.

---

## 🖥️ How to Run the Project

### Prerequisites

1. [Download Unity Hub](https://unity.com/download) and install it.
2. Download and unzip the project repository.

### Opening the Project

1. Open **Unity Hub**.
2. Click **Open Project** and select the extracted project folder.
3. Unity will detect the required version and prompt to install it if necessary.

### Running the Simulation

1. Once the project loads, navigate to:  
   `Assets > Screens`
2. Open the scene file: `GameScreen.unity`
3. Click the **Play** button at the top of the Unity Editor to start the simulation.

---

## 🧠 AI & Fault Injection

- AI behaviors are managed through the `AIPlayerController` script.
- Robots determine their movement based on ball position, teammates, and role zones.
- Injected faults:
  - **Premature Kick Fault** – Simulates a robot kicking too early due to false ball detection.
  - **Player Distance Miscalculation Fault** – Simulates collisions caused by incorrect distance estimation.

---

## 📁 Folder Structure

MechanixLegions/
├── Assets/
│ ├── AI/
│ ├── Screens/
│ ├── Scripts/
│ └── Prefabs/
├── ProjectSettings/
└── README.md

---

## 🧪 Testing & Debugging
- Debug logs are printed to the Unity Console for tracing possession, and goal events.

---

## 📝 Acknowledgments

We thank Professor Kaliappa for his continued support and feedback throughout the semester. This project has strengthened our understanding of AI coordination, low-level networking, and simulation testing.

---

## 📬 Contact

For questions or feedback:
- **Aprajita Srivastava** – aprajitasrivastava@gmail.com  
- **Jezlea Ortega** – jezlea.o@gmail.com
