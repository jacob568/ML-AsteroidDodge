# ML-AsteroidDodge
This project uses Unity ML-Agents to teach a spaceship to collect gold in space, while avoiding asteroids that could destroy it.

## Learning Environment
The Environment is an enclosed cuboid, in which there is no gravity or resistance of any kind, mimicing space. Asteroids are consistently spawned throughout an episode, and fly in the general direction of the spaceship agent. One gold object spawns on either of 2 ends of the cuboid. The agent needs to collect 5 gold to complete an episode, and health reaching 0 will also end the episode.

## The Agent
The spaceship agent has 3 branches of control.
1. Forward and backward motion.
2. Rotation around its X axis.
3. Rotation around its Y axis.

Its view of the world is provided through 3 RayPerceptionSensors, and 14 Vector Observations.
