# battleships
An implementation of Battleships game. 

## Architecture
Based on 3 projects;
+ BlazorApp with Client and Server side related logic.
+ Console app, with an implementation of the game in a terminal.
+ Core library, models and business logic to be reused in a console application and reused between
client and server side blazor.
+ ComputerAlgorihm, library to handle logic if playing vs a computer.

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

## Rules and gameplay
+ Max 2 players allowed
+ Can play vs computer

## How to use
```
$ docker...
```


## Install
+ `

## Screen


