# Tower Defense

C# Windows Forms tower defense game.

## How to run

```
dotnet run
```

Requires .NET 8 SDK.

## Controls

| Key | Action |
|-----|--------|
| Space | Pause |
| Escape | Save and return to menu |
| Mouse | Build towers, select, upgrade |

## Mechanics

5 tower types: tesla, crossbow, cannon, slow, generator. Generators produce energy, other towers consume it. A tower without energy won't fire.

4 enemy types: normal, fast, armored, boss. A boss appears every 10 enemies in a wave.

50 waves. Gems are awarded after victory. Spend gems on global upgrades in the tech menu.

Progress is saved to `save.json`.
