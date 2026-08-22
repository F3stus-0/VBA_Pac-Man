# VBA_Pac-Man
This is a fan-made Pac-Man game created on Visual Basics!

<img width="95" height="95" alt="blinkyright" src="https://github.com/user-attachments/assets/e83884aa-d269-4810-9a3a-9227880ac1d8" /> 
<img width="95" height="95" alt="pinkyright" src="https://github.com/user-attachments/assets/aec6e299-72d8-45ff-85c5-5955f2e0b523" /> 
<img width="95" height="95" alt="inkyright" src="https://github.com/user-attachments/assets/525abaea-84b6-4aff-8a53-6c4f0d7b7919" /> 
<img width="95" height="95" alt="carlosright" src="https://github.com/user-attachments/assets/d6a3dcba-e9b9-43cb-97b1-9436658d9e7e" /> 
<img width="110" height="110" alt="pacmanright" src="https://github.com/user-attachments/assets/bb078b44-7c25-4be2-9d2e-3824eadd435a" /> 

## Demo

https://github.com/user-attachments/assets/bffe15f0-9b59-4d5a-a815-ffb4b6e96a9f


[Play / Download the latest release](https://github.com/F3stus-0/VBA_Pac-Man/releases/tag/v1.0.0)

## Quick Start
- Go to the latest release.
- Download the PACMAN-VBA.zip file.
- Extract the ZIP file.
- Run PACMAN-VBA.exe.
- Use the arrow keys to move Pac-Man.
- Eat all the pellets, avoid the ghosts, and get the highest score!
- ### Requirements: Windows PC. No Visual Studio installation is required.

## Features
- Classic Pac-Man maze
- Pac-Man movement
- Ghosts
- Power pellets
- Fruits
- Score system
- Victory screen

## Running Locally

To run the project from source, you will need:

- Visual Studio 2022 or later
- Visual Basic .NET
- .NET 8.0
- Windows
- No external dependencies or environment variables are required
### Clone the Repository
```bash
git clone https://github.com/F3stus-0/VBA_Pac-Man.git
cd VBA_Pac-Man
```
Open the .sln solution file in Visual Studio.

## How It Works

VBA_Pac-Man is built from scratch in Visual Basic .NET using Windows Forms and .NET 8.0. The game uses a tile-based maze represented by a matrix, allowing the game to determine which spaces are walls, walkable paths, the ghost house, and other special tiles. Pac-Man and the ghosts use grid-based movement and collision detection to interact with the maze.

The project is designed around separate classes for the main game elements, keeping the map, player, ghosts, tiles, and game logic organized independently. Ghost behavior is handled through different states such as chasing, scattering, being frightened, and being eaten. This makes it possible to change how a ghost behaves depending on the current game state rather than relying on one fixed movement pattern.

The game also uses animated sprite assets for Pac-Man and the ghosts. The game loop updates movement, collisions, scoring, ghost behavior, animations, and game states while the Windows Forms drawing system renders the current state of the maze and characters.

## Credits / Acknowledgements

- **Pac-Man** was originally created by Namco. This project is a fan-made implementation created for educational purposes and is not affiliated with or endorsed by Namco.
- Built with **Visual Basic .NET** and **.NET 8.0**.
- Built using **Visual Studio**.
- Some visual assets are based on the original Pac-Man visual style and are used for this fan-made educational project.
- Thanks to the open-source community and documentation that helped with learning Visual Basic .NET, Git, and game development.
