# What is this?
An implementation of Battleships game. Primarily to try out and learn Blazor Webassembly.
Can play vs another player, or vs computer, which can play the game with 4 options of difficulty.
See below section for that. 

## Install
+ Download source code and compile BlazorApp.Server.csproj
+ Pull as docker image from Docker Hub: ```docker pull elpulgo/battleships-blazor:latest```
+ If using docker-compose, use `docker-compose-prod.yml` to start container

## Architecture
Based on 3 projects;
+ BlazorApp with Client and Server side related logic.
+ Console app, with an implementation of the game in a terminal.
+ Core library, models and business logic to be reused in a console application and reused between
client and server side blazor.
+ AI-lib, library to handle logic if playing vs a computer.

The design is based on an event driven approach, where the user do an action, a GameManager handles
the state of the game, and based on the outcome of the action, events are then invoked to the players of the game.

There is a notificationservice which is used to fire events from Server side Blazor, wrapped around SignalR.

Client side, a message-service is listening on the events fired by the server, and pass on the result to an Event service,
which invoke eventhandlers, primarily used by the Index page in client side blazor.
The index page updates gameboards for the player, and other necesary information, which is used as cascading values, so the different
components can utilize the global state of the game.

I have tried to use a single flow of control, from serverside to eventhandlers via messageservice, which relays to eventservice, rather
than each component should listen in on the events, or do requests to the server to get information of the state.

This gives a cleaner perception of the actual state of the game and is easier to reason with from a coding perspective.

## Algorithms & Benchmark

When playing vs the computer, there are 4 possible levels of difficulty.
- Random, as the name suggest, fire random shots across the board.
- Hunter, if a coordinate is hit, try and find neighbour coordinates until the ship is destroyed. Fallback to random if no hits available per round.
- MonteCarlo, use a probability simulation, simulating 20 random gameboards for all ships per round, where 1 score is 1 coordinate where a ship exist. Then take the coordinate with the highest score.
- MonteCarloWithHunt, as MonteCarlo, but will fallback to Hunt when any hits occur.

Benchmark for 1000 iterations of gameplay when playing as AI for the different prediction algorithms.

The test starts with 100 in score(all available boxes), which is not theoretically possible since we have 
```
Aircraftcarrier 	5
Battleship 	 	4
Cruiser		 	3
Destroyer	 	2
Destroyer	 	2
Submarine	 	2
Submarine	 	2
------------------
Total 			20
```

That is a total of 20 boxes which need to be marked before the game can end.
So practical max score is 80.
Min score would be 0 if we need to hit exactly all boxes before we find the last marked ship box.

Below is the score for the different algorithms based on 1000 iterations.

|           | Min                        | Average                 | Max                                        |
| --------- | :------------------------------------------:     | :----------------------:                 | :---------------------------:                            | 
| Random|0|3.86|24|
| Hunter|0|29.96|63|
| MonteCarlo|23|42.83|65|
| MonteCarloWithHunt|22|52.48|75|

## Rules and gameplay
+ Max 2 players allowed
+ Can play vs computer
+ Board consist of 10 * 10 boxes, with 7 ships, with a total of 20 boxes.
+ Enter player name, and select to play vs player or computer
+ Wait for other player to join game on same local ip
+ Setup the board, either through selecting coordinates yourself, or by randomly generating the ships.
+ Play the game, first to sink opponent ships win

## Screen

<img src="https://github.com/Elpulgo/battleships/blob/master/screens/setup.png" width="640">
<img src="https://github.com/Elpulgo/battleships/blob/master/screens/gameplay.png" width="640">
<img src="https://github.com/Elpulgo/battleships/blob/master/screens/gameplay2.png" width="640">
<img src="https://github.com/Elpulgo/battleships/blob/master/screens/final.png" width="640">



