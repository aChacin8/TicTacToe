# TIC TAC TOE

Este proyecto es una aplicación web del juego de "El Gato" donde el usuario se enfrenta a una Máquina con lógica de estados.

## 1. Explicación del Código

El sistema utiliza una arquitectura **Cliente-Servidor**:
* **Backend (C#):** Expone un endpoint (`/next-move`) que recibe el tablero actual. Implementa una búsqueda de un solo nivel que prioriza ganar, luego bloquear al usuario y finalmente ocupar posiciones estratégicas (Centro > Esquinas). Incluye un factor de aleatoriedad del 10% para permitir victorias del usuario.
* **Frontend (React):** Maneja la interfaz gráfica y el estado visual. Calcula y muestra una barra de "Probabilidad de Victoria" basada en las líneas que el usuario aún puede completar según el estado enviado por el backend.

## 2. Instalación y Ejecución

### Requisitos previos
* .NET SDK 8.0
* Node.js 

### Pasos para el Backend (API)
1. Navega a la carpeta del proyecto backend: `cd tic-tac-toe/TicTacToe_Api/TicTacToe_Api`.
2. Restaura las dependencias: `dotnet restore`.
3. Ejecuta la API: `dotnet run`.
4. La API estará disponible en `http://localhost:5000` o `http://localhost:5173`.

### Pasos para el Frontend (React)
1. Navega a la carpeta del frontend: `cd tic-tac-toe/frontend `.
2. Instala los paquetes: `npm install`.
3. Inicia la aplicación: `npm start`.
4. Abre ` http://localhost:5173` en tu navegador.

## 3. Características Clave
* **Inicio Aleatorio:** La Máquina siempre realiza el primer movimiento en una posición aleatoria, pudiendo ser al centro o en las esquinas.
* **Heurística:** La Máquina evalúa el tablero en tiempo real para decidir entre atacar o bloquear al usuario.
* **Probabilidades:** Se maneja un sistema de probabilidad de victoria que orienta al usuario sobre que tanto exito podría alcanzar en su siguiente jugada.

