# Autonomous Car Simulation(Agent-based Model) Using Unity

Tools & Language: C#, Unity2D

## Agent

- Car object placed in the race track
- 4 wheels to actuate
- 3 sensors to percept the environment

![agent car](/images/car-agent.png)

## Sensors

- Raycast as 3 sensors attached to the car object:
  - Front-left
  - Front
  - Front-right
- Percept the environment
- Indicate the distance between the Agent and the obstacles

![sensors](/images/sensors.png)

## Test Case

- Our first race track was done by pixel art, which has a sharp turn.

![Test Case 1](/images/demo.gif)

- The final race track used a regular race track picure that we found online. We added edge colliders to the side of the race track to listen incoming collisions

![Race Track](/images/race-track.png)

- Here is an demo showcasing the car agent driving in the race track without any collision after self-trained about 20 generations

![Test Case 2](/images/demo2.gif)