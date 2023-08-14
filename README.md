# Lobbies_NGO

This project consists of a multiplayer lobby, where players can create a lobby, or join an existing lobby.
Main Packages included in the project: Authentication, Lobby, Relay, ParrelSync (this allows the use of multiple editors and player profiles without the need to build the game).

In this project, a lobby can contain up to 4 players and includes a player name, a player avatar, Kick buttons, a leave lobby button, and some maps to cycle through and load when clicking the "START" button.
The lobby host is the only one who has control over the following elements: Kick player, change map, start the game.
Clients inside a lobby who are not the host are allowed to change their player avatar image.

For a player client who want to join a lobby, they can either join using a "Lobby Id" from a lobby list, a "Lobby Code" or a quick join buttons.
