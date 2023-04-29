# Like Moths to a Star

## TODO

- [x] 💚 MVP idea - a side-scrolling 2d pixel art game, mostly focused on story
    - There's a bit of strategy involved - the player chooses which route to take; they're free to take the path to the
      sun immediately, but it will be very difficult or maybe impossible. Instead, they have to take paths to nearby
      celestial bodies (asteroids, moons, planets) to collect resources for upgrades first.
        - After a trip to a celestial body is over, the moth turns into a cocoon and the player can choose upgrades
          based on the collected resources.
        - (Just cosmetic, not a part of the MVP) At each celestial body, the player is shown what it looks like,
          with [lunar creatures](https://assets1.ignimgs.com/2008/11/20/elebits-the-adventures-of-kai-and-zero-20081120034550582-2655629.jpg)
          and [solar creatures](https://assets2.ignimgs.com/2006/05/10/elebits-20060509074340714-1497107.jpg) inhabiting
          it, perhaps.
    - While travelling, the player has to dodge debris and enemy projectiles, but since the focus is on the story, it
      won't be too challenging.
    - Boss fights when a certain difficulty is reached. The player starts at the bottom-left and has to reach the top-left (where the Sun is) and the difficulty increases diagonally in that direction... like this:
  ![solar-game-path-concept.png](../../blob/main/solar-game-path-concept.png?raw=true)

### MVP

- [ ] 💙 Main menu - celestial map scene (this will also serve as the main menu)
- [ ] 💙 Going from one star to another - flight scene
- [ ] 💙 Player in the flight scene
- [ ] 💙 The flight scene has a timer/distance meter, the length of which depends on the length of the path
- [ ] 💙 HP (replenished upon arrival)
- [ ] 💙 Obstacles
- [ ] 💙 Suncake pickups
- [ ] 💙 Shoot to destroy obstacles
- [ ] 💙 Button that takes the player from the map scene to the upgrade scene (which is just )
- [ ] 💙 Fire speed upgrade
- [ ] 💙 Fire power upgrade
- [ ] 💙 Max HP upgrade
- [ ] 💙 When the Sun is reached, just say the player has won in the celestial map
- [ ] 💟 Publish `0.1.0`

### Basic features

- [ ] 💙 Show some info when navigating in the celestial map
- [ ] 💙 Limited ammo
- [ ] 💙 Ammo count upgrade
- [ ] 💙 Bullet speed upgrade
- [ ] 💙 Auto-saving/auto-loading
- [ ] 💜 Celestial map environment background
- [ ] 💜 Flight scene environment background
- [ ] 💜 Celestial body sprite(s)
- [ ] 💜 Projectile sprite
- [ ] 💜 Suncakes
- [ ] 💛 Choose flight destination SFX
- [ ] 💛 Navigate to/from upgrade scene SFX
- [ ] 💛 Teleport to beginning SFX
- [ ] 💛 Clear level SFX
- [ ] 💟 Publish `0.2.0`

### Advanced features

- [ ] 💙 Firefly enemies
- [ ] 💙💜 When the player reaches the sun, they can choose to explore it, leading to a special flight scene with story
- [ ] 💜 Upgrade scene environment background
- [ ] 💜 Upgrade scene creatures
- [ ] 💙 Reversing time (going to the last visited celestial body, reversing progress)
- [ ] 💙💜 Level icons
- [ ] 💛 Shoot SFX
- [ ] 💛 Celestial map music
- [ ] 💛 Upgrade scene music
- [ ] 💛 Flight music
- [ ] 💛 Sun flight music
- [ ] 💛 Hovering over celestial body SFX
- [ ] 💜 Cover art
- [ ] 💟 Publish `0.3.0`

### Expert features

- [ ] 💜 Icon
- [ ] 💙💚 Bosses; dialogue with bosses
- [ ] 💟 Publish `0.4.0`

---

#### Legend

- 💙 Code/Godot
- 💜 Art
- 💚 Design
- 💛 Audio
- 💟 Special
