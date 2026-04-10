# 🎵 Trilha dos Sons — ASP.NET Core + SignalR

## 📁 Estrutura do Projeto

```
TrilhaDosSons/
├── TrilhaDosSons.csproj
├── Program.cs
├── Hubs/
│   └── GameHub.cs
├── Models/
│   └── GameModels.cs
└── wwwroot/
    ├── index.html
    └── sons/          ← coloque os .mp3 aqui
```

## ▶ Rodar localmente
1. Abra o `TrilhaDosSons.csproj` no Visual Studio 2022
2. Pressione **F5**
3. Acesse `http://localhost:5000`

## 🚀 Publicar no IIS

**Pré-requisito:** instale o [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)
(instala o runtime + módulo ASP.NET Core para o IIS automaticamente)

**No Visual Studio:**
1. Botão direito no projeto → **Publicar**
2. Escolha **Pasta** → destino: `C:\inetpub\wwwroot\TrilhaDosSons`
3. Clique em **Publicar**

**No Gerenciador do IIS:**
1. Adicionar Site → aponte para `C:\inetpub\wwwroot\TrilhaDosSons`
2. Pool de Aplicativos → **.NET CLR: Sem código gerenciado** | Pipeline: Integrado
3. Habilitar WebSocket: Recursos do Windows → IIS → Recursos de Desenvolvimento → ✅ WebSocket Protocol

## 🔊 Áudios (pasta wwwroot/sons/)
```
cachorro.mp3  gato.mp3    vaca.mp3     galo.mp3    pato.mp3
ovelha.mp3    porco.mp3   sapo.mp3     passaro.mp3 cavalo.mp3
elefante.mp3  leao.mp3    relogio.mp3  sino.mp3    abelha.mp3
trem.mp3      piano.mp3   flauta.mp3   tambor.mp3  telefone.mp3
```
Baixe grátis em: freesound.org ou pixabay.com/sound-effects

## 🔄 Node.js → ASP.NET Core
| Antes (Node.js)   | Agora (C#)              |
|-------------------|-------------------------|
| socket.io         | SignalR                  |
| server.js         | Program.cs + GameHub.cs  |
| npm install       | NuGet (automático)       |
| node server.js    | F5 no Visual Studio      |
