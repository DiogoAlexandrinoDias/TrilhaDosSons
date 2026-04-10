# 🎵 Trilha dos Sons — versão C# / ASP.NET Core + SignalR

## 📁 Estrutura do projeto

```
TrilhaDosSons/
├── TrilhaDosSons.csproj
├── Program.cs
├── Hubs/
│   └── GameHub.cs          ← toda a lógica do jogo (equivalente ao server.js)
└── wwwroot/
    ├── index.html           ← front-end completo (usa SignalR em vez de Socket.io)
    └── sons/                ← coloque os .mp3 aqui
        ├── cachorro.mp3
        ├── gato.mp3
        └── ...
```

---

## ▶ Como rodar no Visual Studio

1. Abra o arquivo `TrilhaDosSons.csproj` no Visual Studio 2022
2. Clique em **▶ IIS Express** ou **▶ https**
3. Acesse `http://localhost:PORT`

---

## 🌐 Como publicar no IIS

### 1. Publicar pelo Visual Studio
- Clique com botão direito no projeto → **Publicar**
- Escolha **Pasta** (ou diretamente **IIS**)
- Clique em **Publicar**

### 2. Configurar o IIS
No IIS Manager:
- Crie um novo **Site** apontando para a pasta publicada
- **Application Pool** → altere para **"No Managed Code"**
  (ASP.NET Core roda fora do IIS, o IIS só faz proxy)
- Instale o **ASP.NET Core Hosting Bundle**:
  https://dotnet.microsoft.com/download/dotnet/8.0
  (seção "Hosting Bundle")

### 3. Pronto!
Outros computadores na rede acessam pelo IP da máquina:
```
http://SEU_IP:PORTA
```

---

## 🔊 Áudios necessários (pasta wwwroot/sons/)

```
cachorro.mp3  gato.mp3    vaca.mp3     galo.mp3    pato.mp3
ovelha.mp3    porco.mp3   sapo.mp3     passaro.mp3 cavalo.mp3
elefante.mp3  leao.mp3    relogio.mp3  sino.mp3    abelha.mp3
trem.mp3      piano.mp3   flauta.mp3   tambor.mp3  telefone.mp3
```

Baixe grátis em: freesound.org ou pixabay.com/sound-effects

---

## 🔄 Diferenças vs versão Node.js

| Node.js          | C# / ASP.NET Core       |
|------------------|-------------------------|
| `socket.io`      | `SignalR`               |
| `server.js`      | `GameHub.cs`            |
| `socket.emit()`  | `connection.invoke()`   |
| `socket.on()`    | `connection.on()`       |
| `io.to('sala')`  | `Clients.Group("sala")` |
| `npm install`    | NuGet automático        |

