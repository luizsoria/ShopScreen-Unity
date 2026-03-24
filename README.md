# Shop Screen - Unity UI Toolkit

A shop/store UI screen implementation for Unity using **UI Toolkit** (UXML/USS), following a provided Figma design. This project demonstrates a functional in-game shop with coin and money purchase options, built with clean architecture and SOLID principles.

## Overview

This project implements a simple Shopping menu for purchasing digital coins using real money (In-App Purchases mock). The UI is built entirely with **UI Toolkit** (UXML and USS), without using the legacy UI system (GameObjects/Canvas).

## Features

- **Three shop tabs**: Offers, Money, and Coins
- **Procedural item loading**: Shop items are loaded from ScriptableObject data via C# scripts
- **Reusable components**: UXML templates and USS styles designed for reusability
- **Mock purchases**: Purchase actions are simulated with Debug.Log
- **Landscape orientation**: All UIs are designed for landscape mode
- **SOLID principles**: Clean separation of concerns with interfaces and dependency injection

## Architecture

The project follows SOLID principles with a clear separation of concerns.

### Data Layer
The data layer uses ScriptableObjects to define shop items and catalog configuration. `ShopItemData` represents individual purchasable items, while `ShopCatalog` organizes items by category (Offers, Money, Coins).

### Service Layer
An `IShopService` interface defines the contract for purchase operations, with `MockShopService` providing a debug implementation. Similarly, `IWalletService` manages the player's currency balances.

### UI Layer
The UI layer extends the provided `UIScreenBase` class. `ShopScreen` orchestrates the entire shop view, delegating to specialized component controllers for tabs, item cards, and wallet displays.

### UI Toolkit Components
All visual elements are built with UXML templates and USS stylesheets, organized as reusable components that can be instantiated procedurally.

## Project Structure

```
Assets/
├── Art/UI/Shop/          # Image assets (sprites, icons, backgrounds)
├── Scripts/
│   ├── Common/UI/        # Base UI classes (UIScreenBase, UINavigationController)
│   └── Shop/
│       ├── Data/         # ScriptableObjects for shop configuration
│       ├── Services/     # Service interfaces and mock implementations
│       └── UI/           # UI controller scripts
├── UI/
│   ├── UXML/             # UI Toolkit layout files
│   │   └── Components/   # Reusable UXML templates
│   └── USS/              # UI Toolkit style files
│       └── Components/   # Component-specific styles
└── Resources/ShopData/   # Runtime-loadable shop data
```

## Requirements

- **Unity 6000.3 LTS** (Unity 6.3 LTS) or later
- UI Toolkit package (included by default)

## Setup

1. Clone this repository
2. Open the project in Unity 6000.3 LTS
3. Open the `ShopScene` scene from `Assets/Scenes/`
4. Enter Play Mode to interact with the shop UI

## Design Reference

The UI follows the Figma design provided in the test specification, implementing all three tabs (Offers, Money, Coins) with their respective layouts and visual styles.
