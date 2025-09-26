# Overview

It is a simple console-based chat application that utilizes UDP for message transmission and RSA encryption for secure communication. It was build for learning purposes.

---

### Usage

Upon running the application, you will see a menu with the following commands:

/q: Quit the application.

/x: Change the destination IP address.

/c: Clear the chat history.

/r: Reset the encryption key.

/t: Open/Close TicTacToe view

/request: Send your public key to the specified IP address.

---

### Example Usage

1. Communication without encryption:

Type /x -> set recipient IP -> type anything -> user receives your message

2. Encrypted communication:

Type /x -> set recipient IP -> type /request (this sends the recipient your public key, he needs to confirm it with /confirm) -> when he sends you message it will be encrypted (if you want to have your messages encrypted as well, he needs to send you his public key and you need to accept it with /confirm)

3. Playing TicTacToe:

Type /t to enable TicTacToe view -> type "TTT x y" to make move -> wait until opponent makes his move -> continue playing
