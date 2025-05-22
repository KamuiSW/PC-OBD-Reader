# ROADMAP.md

## 🚗 OBD2 Windows App Roadmap

A Windows desktop app that connects to a Bluetooth OBD2 reader, displays real-time car data with graphs, and saves data for later analysis.

---

## 🏁 Phase 1: Setup & Planning

- [x] Decide programming language: **C# with .NET (WPF)**
- [x] Choose libraries:
  - Bluetooth: `32feet.NET`
  - Graphing: `LiveCharts2`
  - File I/O: built-in .NET I/O
- [ ] Plan UI layout (e.g. connect button, live graph, data log)

---

## 🔌 Phase 2: Bluetooth Connection

- [ ] Use `32feet.NET` to scan for Bluetooth devices
- [ ] Identify and connect to the OBD2 reader
- [ ] Handle basic connection errors (timeout, pairing issues)

---

## 🧪 Phase 3: OBD2 Data Handling

- [ ] Send standard OBD2 PIDs (e.g. RPM, speed, coolant temp)
- [ ] Receive and parse responses
- [ ] Structure data into usable format (timestamp + value)

---

## 📈 Phase 4: Data Visualization

- [ ] Setup `LiveCharts2` with real-time updating charts
- [ ] Create toggleable graphs (RPM, speed, etc.)
- [ ] Smooth performance for live updates

---

## 💾 Phase 5: Data Saving

- [ ] Save session data to CSV or JSON
- [ ] Add filename auto-generating with timestamps
- [ ] Load previous sessions for analysis

---

## 🧼 Phase 6: UI Polishing

- [ ] Improve layout with WPF styles
- [ ] Add status indicators (connected, receiving data, error)
- [ ] Add user settings (e.g. refresh rate, preferred sensors)

---

## 📦 Phase 7: Packaging & Testing

- [ ] Build standalone EXE with .NET
- [ ] Test on multiple Windows PCs
- [ ] Write basic user instructions

---

## ✅ Future Ideas

- [ ] Export to Excel
- [ ] Custom alerts (e.g. high RPM warning)
- [ ] GPS + map overlay (if GPS is available)
- [ ] Wi-Fi or USB OBD2 support

---

## 📚 Tech Stack Summary

- **Language:** C#
- **Framework:** .NET (WPF)
- **Bluetooth:** 32feet.NET
- **Graphing:** LiveCharts2
- **UI:** XAML (WPF)
- **Data format:** CSV / JSON

