# Networked 2D Shooter Game with Unity's Netcode for GameObjects

## Overview of the Game Design
The project is a simple networked multiplayer game where players control avatars in a 2D environment. The players can move, shoot projectiles, and interact with objects. The game supports at least two players, functioning as a host-client model.

## Implementation Details
### Player Movement Synchronization
 - Implemented using the `ClientNetworkTransform` class to allow smooth client-authoritative movement updates.

### Projectile Synchronization
 - Handled through the `ShooterComponent` class, which spawns bullets on the server and synchronizes their behavior with clients using `ServerRpc`.

### Health System
 - Utilizes a `NetworkVariable` to synchronize player health changes across the network.

### Chat System
 - A `ChatManager` facilitates in-game communication. Messages are sent using `ServerRpc` and broadcasted via `ClientRpc`.

## Features
- Smooth player movement synchronization
- Real-time projectile spawning and synchronization
- In-game chat system for player communication
- Health management system with real-time updates

## Challenges and Solutions
The most challenging aspect was ensuring proper synchronization of networked objects, particularly projectiles. This was resolved by:
- Leveraging server authority to handle object spawning and destruction.
- Using `NetworkObject` components for managing object lifecycle across clients.
