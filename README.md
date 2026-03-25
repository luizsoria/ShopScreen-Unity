# Shop Screen - Unity UI Toolkit

A shop/store UI screen implementation for Unity using **UI Toolkit** (UXML/USS), following a provided Figma design. This project demonstrates a functional in-game shop with coin and money purchase options, built with clean architecture and SOLID principles.

## Overview

This project implements a Shopping menu for purchasing digital coins using real money (In-App Purchases mock). The UI is built entirely with **UI Toolkit** (UXML and USS), without using the legacy UI system (GameObjects/Canvas).

## Features

- **Three shop tabs**: Offers, Money, and Coins with animated tab transitions
- **Procedural item loading**: Shop items are loaded from ScriptableObject data via C# scripts
- **Reusable components**: UXML templates and USS styles designed for reusability
- **Purchase confirmation popup**: Modal dialog with item details before completing a purchase
- **Purchase feedback toast**: Animated success/error notifications after purchases
- **Offer countdown timers**: Live countdown timers on offer cards creating urgency
- **Wallet balance animation**: Animated number transitions when balance changes
- **Ribbon badges**: "BEST VALUE" and "POPULAR" badges on offer cards
- **Staggered animations**: Cards slide in with staggered delays for polished feel
- **Mock purchases**: Purchase actions are simulated with Debug.Log and EventBus events
- **Landscape orientation**: All UIs are designed for landscape mode
- **SOLID principles**: Clean separation of concerns with interfaces and dependency injection

## Architecture

The project follows SOLID principles with a clear separation of concerns.

### Data Layer
The data layer uses ScriptableObjects to define shop items and catalog configuration. `ShopItemData` represents individual purchasable items, while `ShopCatalog` organizes items by category (Offers, Money, Coins).

### Service Layer
An `IShopService` interface defines the contract for purchase operations, with `MockShopService` providing a debug implementation. Similarly, `IWalletService` manages the player's currency balances. Services publish events via `EventBus` for decoupled communication.

### Infrastructure Layer
- **EventBus**: Type-safe publish/subscribe event system for decoupled communication between UI components
- **ServiceLocator**: Lightweight dependency injection container for service resolution
- **UIAnimationHelper**: Reusable animation utilities (fade, slide, scale bounce, pulse, number animation)

### UI Layer
The UI layer extends the provided `UIScreenBase` class (with a fix for proper callback cleanup). `ShopScreen` orchestrates the entire shop view, delegating to specialized component controllers:

- **ShopItemCardController**: Individual item cards for Money/Coins tabs
- **OfferCardController**: Offer cards with timers and ribbon badges
- **WalletDisplayController**: Header wallet displays with animated balance updates
- **ConfirmationPopupController**: Purchase confirmation modal
- **PurchaseFeedbackController**: Success/error toast notifications
- **OfferTimerController**: Countdown timer for time-limited offers

### UI Toolkit Components
All visual elements are built with UXML templates and USS stylesheets, organized as reusable components that can be instantiated procedurally.

## Project Structure

```
Assets/
├── Art/UI/Shop/              # Image assets (sprites, icons, backgrounds)
├── Scenes/                   # Unity scenes
│   └── ShopScene.unity       # Main shop scene (included in Build Settings)
├── Scripts/
│   ├── Common/
│   │   ├── DI/               # ServiceLocator for dependency management
│   │   ├── Events/           # EventBus for decoupled communication
│   │   └── UI/               # Base UI classes, animation helpers
│   └── Shop/
│       ├── Data/             # ScriptableObjects for shop configuration
│       ├── Events/           # Shop-specific event definitions
│       ├── Services/         # Service interfaces and mock implementations
│       ├── UI/               # UI controller scripts
│       └── Editor/           # Editor tools for scene setup
├── UI/
│   ├── UXML/                 # UI Toolkit layout files
│   │   └── Components/       # Reusable UXML templates
│   └── USS/                  # UI Toolkit style files
│       └── Components/       # Component-specific styles
└── Resources/ShopData/       # Runtime-loadable shop data (generated via Editor tools)
```

## Requirements

- **Unity 6000.3.12f1 LTS** (Unity 6.3 LTS)
- UI Toolkit package (included by default)

## Quick Start

Follow these steps **in order** after cloning the repository:

1. **Clone** this repository
2. **Open** the project in **Unity 6000.3.12f1 LTS**
   - If prompted about "Corrupted Library", click **Rebuild Library**
   - If prompted about "Project Change Required", click **Continue**
   - If prompted about "Enter Safe Mode" due to compilation errors, click **Ignore** (errors should resolve after full import)
   - If prompted about "Mobile Dependency Resolver", click **Ignore**
3. **Wait** for Unity to finish importing all assets and compiling scripts
4. Run **Tools > Shop > Create Default Catalog** to generate the ScriptableObject data
5. Run **Tools > Shop > Setup Shop Scene** to create and configure the scene
   - This creates the scene with all GameObjects, assigns UXML/USS/PanelSettings, and adds it to Build Settings
6. **Enter Play Mode** to interact with the shop UI

> **Note**: Steps 4 and 5 are required because ScriptableObject assets and scene references contain GUIDs that are generated per-project. The Editor tools automate the entire setup process.

## Purchase Flow

1. User clicks a "Buy" button on any item or offer card
2. A **confirmation popup** appears with item details and price
3. User confirms or cancels the purchase
4. On confirmation, the mock service processes the purchase
5. A **feedback toast** appears showing success/error
6. The **wallet balance** animates to reflect the new amount

## Design Reference

The UI follows the Figma design provided in the test specification, implementing all three tabs (Offers, Money, Coins) with their respective layouts and visual styles.

[Figma Design](https://www.figma.com/design/yx64FCOuxUWWEsEHidgIho/Shop-Screen---Test)
