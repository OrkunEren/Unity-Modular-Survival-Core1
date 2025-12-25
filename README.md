README.md
# Unity Modular Survival Core
### Scalable, Event-Driven Survival Engine Prototype

![Unity](https://img.shields.io/badge/Unity-6.2%2B-black?style=flat&logo=unity)
![C#](https://img.shields.io/badge/C%23-10.0-blue?style=flat&logo=csharp)
![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-green)
![Status](https://img.shields.io/badge/Status-Prototype-orange)

## 📖 Overview

**Unity Modular Survival Core** is a robust backend framework designed for survival games, focusing on **scalability, clean architecture, and performance**. 

Unlike typical "tutorial-based" projects, this system is engineered with a **Hybrid Architecture** combining Singleton patterns for high-level management and an Event-Driven system to decouple logic. It features a realistic weather simulation that directly impacts gameplay mechanics (physics, audio, and player stats) through dynamic data binding.

---

## ⚙️ Technical Architecture

This project strictly adheres to **SOLID principles**, specifically the **Single Responsibility Principle (SRP)**.

### 1. Hybrid Manager Pattern
* **Decoupled Systems:** The `WeatherManager` does not directly control the `AudioManager`. Instead, systems communicate via **Events**.
* **State Management:** Vital stats (Health, Hunger, Hydration) are managed via a central state machine but broadcast changes to UI and Audio subsystems asynchronously.

### 2. Event-Driven Communication
Instead of using tightly coupled `Update()` loops checking for conditions every frame, the project uses C# Events:
```csharp
// Example of the decoupled approach used in this project
public event Action<float> OnRainIntensityChanged;

// AudioManager listens to this event, eliminating the need for a direct reference to WeatherManager

3. Optimization Techniques
Raycast-Based Surface Detection: Footstep sounds and physics interactions are calculated dynamically based on surface texture and current weather conditions (Wet vs. Dry).

Procedural Weather Generation: Rain and snow cycles use randomized intervals with weighted probabilities, ensuring organic transitions rather than predictable loops.

🛠️ Key Features
🌧️ Dynamic Weather & Environment System
Atmospheric Control: Seamless transitions between day/night cycles, affecting fog density and volumetric lighting via Mathf.Lerp.

Physical Interaction: Snow accumulation logic implemented via Shader manipulation and particle system adaptation.

Reactive Audio: An advanced audio middleware layer that mixes "Surface Type" (Grass, Concrete) with "Weather State" (Wet, Snowy) to produce realistic feedback.

❤️ Vital Stats & Survival Mechanics
Context-Aware Feedback: When health drops below 30%, the system triggers a heartbeat audio cue via the Event System.

Environmental Impact: Proximity to fire sources dynamically calculates temperature rise and moisture reduction rates.

🎒 Modular Inventory System
ScriptableObject Architecture: Items are defined as data assets, allowing for easy expansion without code changes.

Inventory Logic: Completely separated from UI logic to ensure easier unit testing and maintenance.

📝 Development Process & Code Quality
I approach game development with an Engineering Mindset:

Planning: All systems are diagrammed on whiteboards before coding to establish correct class relationships.

Naming Conventions: Variables and functions follow strict self-documenting naming rules (e.g., HandleMovement() vs CalculateGravity()).

Git Flow: The project is managed using industry-standard Git Flow (Feature branches -> Develop -> Main).

⚠️ Asset Disclaimer
This project is intended for educational and prototyping purposes only.

Audio Assets: Third-party sound effects are used as placeholders to demonstrate the AudioManager functionality. All rights belong to their respective owners.

Commercial Use: This codebase is not intended for commercial release in its current state containing these assets.

🚀 Getting Started
Clone the repository:

Bash

git clone [https://github.com/OrkunEren/Unity-Modular-Survival-Core.git](https://github.com/OrkunEren/Unity-Modular-Survival-Core.git)
Open in Unity 6 (6000.0+) or later.

Open the MainScene to observe the WeatherManager in action.

Developed by a Computer Engineering Student passionate about System Architecture in Games.
