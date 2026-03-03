# Idle Space Vertical Slice Setup

## Unity Version
- Recommended: **Unity 2022.3 LTS** (2D Core template).

## 1) Project Folder/Import Prep
The repository contains raw source art/audio under `/assets`. Copy/import these into Unity's `Assets` folder:

1. In Unity Project window create:
   - `Assets/Art`
   - `Assets/Audio`
2. Drag files from repo:
   - `assets/sprites/**` and `assets/backgrounds/**` into `Assets/Art`
   - `assets/sfx/background.mp3`, `click.ogg`, `buy.ogg`, `prestige.ogg` into `Assets/Audio`
3. Keep sprite wiring via Inspector (no filename hardcoding in code).

## 2) Generate Balance ScriptableObjects
1. Let scripts compile.
2. Run menu: **IdleSpace → Create Default Balance Assets**.
3. This creates:
   - `Assets/ScriptableObjects/Balance/GameBalanceSettings.asset`
   - `Assets/ScriptableObjects/Balance/WeaponDefinition_01..10.asset`
4. (Optional) Edit values for balancing in Inspector.

## 3) Create Scene and Core Objects
1. Create/open scene: `Assets/Scenes/Game.unity`
2. Main Camera:
   - Projection: Orthographic
   - Position Z: -10
3. Create `Ship` GameObject at world `(0,0,0)`:
   - `SpriteRenderer` (assign ship sprite)
   - `CircleCollider2D` (isTrigger=true)
   - `Ship` script
4. Create empty `Systems` GameObject and add:
   - `PortraitBootstrap`
   - `SaveManager`
   - `EconomyManager`
   - `WeaponManager`
   - `AudioManager`
   - `EnemySpawner`
   - `WaveManager`
   - `CombatSystem`
   - `GameManager`

## 4) Prefabs
### Enemy.prefab (`Assets/Prefabs/Enemies/Enemy.prefab`)
- Components:
  - `SpriteRenderer` (assign enemy sprite)
  - `Rigidbody2D` (Body Type: Kinematic)
  - `CircleCollider2D` (isTrigger=true)
  - `Enemy`

### Projectile.prefab (`Assets/Prefabs/Projectiles/Projectile.prefab`)
- Components:
  - `SpriteRenderer`
  - `CircleCollider2D` (isTrigger=true)
  - `Projectile`

### GoldDropFX.prefab
- Simple `ParticleSystem` prefab (or animated sprite) and assign to `EconomyManager.goldDropFxPrefab`.

## 5) Object Pools
1. Create `EnemyPool` GameObject with `ObjectPool`.
2. Create `ProjectilePool` GameObject with `ObjectPool`.
3. In each pool component assign prefab and initial size.

## 6) Wire Inspector References
### GameManager
- balanceSettings: `GameBalanceSettings.asset`
- ship: `Ship`
- waveManager: `WaveManager`
- weaponManager: `WeaponManager`
- economyManager: `EconomyManager`
- saveManager: `SaveManager`
- audioManager: `AudioManager`

### WaveManager
- balance: `GameBalanceSettings.asset`
- enemySpawner: `EnemySpawner`
- enemyPool: `EnemyPool`
- ship: `Ship`
- economy: `EconomyManager`
- combatSystem: `CombatSystem`

### CombatSystem
- ship: `Ship`
- weaponManager: `WeaponManager`
- balanceSettings: `GameBalanceSettings.asset`
- projectilePool: `ProjectilePool`

### WeaponManager
- weaponDefinitions: assign all 10 WeaponDefinition assets in order.

### AudioManager
- musicSource: AudioSource (loop enabled)
- sfxSource: AudioSource (one shot)
- backgroundMusic: `background.mp3`
- clickClip: `click.ogg`
- buyClip: `buy.ogg`
- prestigeClip: `prestige.ogg`

## 7) UI Setup (Portrait)
1. Add Canvas (Screen Space - Overlay) + CanvasScaler (Scale With Screen Size, e.g., 1080x1920).
2. Optional: Add `SafeAreaFitter` to root panel.
3. Top HUD:
   - `TMP_Text` for Wave label
   - HP bar background + fill `Image`
   - `TMP_Text` Gold label
4. Bottom panel:
   - `ScrollView`
   - Content root for rows
5. Create WeaponRow prefab (`Assets/Prefabs/UI/WeaponRow.prefab`) with:
   - Icon `Image`
   - Name/Level/DPS/Cost `TMP_Text`
   - Buy `Button`
   - `WeaponRowUI` script wired to children
6. Add `UIController` on Canvas and wire managers + UI references + row prefab.

## 8) How to Play
- Press Play.
- Ship stays centered and auto-fires nearest enemy.
- Enemies spawn from outside camera bounds.
- Kill all enemies to advance wave.
- Every 10th wave spawns one boss (scaled enemy with boss multipliers).
- If ship HP reaches 0 during a wave:
  - wave resets to `lastClearedWave`
  - ship HP resets to max
  - enemies/projectiles clear
  - gold remains
- Buy weapon levels in UI to increase total DPS.
- Save is automatic via PlayerPrefs JSON.

## Notes / TODO Hooks
- Prestige, offline progress, and enemy variety are left as TODO markers in `GameManager`.
