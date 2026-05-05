# RedeSocial — Frontend

Interface web estilo Facebook para a API RedeSocial (.NET 8).

## Como usar

### 1. Suba a API
Certifique-se de que a API está rodando em `http://localhost:5000`.

### 2. Abra o frontend
Abra o arquivo `frontend/index.html` diretamente no navegador, **ou** sirva com qualquer servidor estático:

```bash
# Python (mais simples)
cd frontend
python -m http.server 3000

# Node.js (npx)
cd frontend
npx serve .
```

Acesse: `http://localhost:3000`

### 3. Configurar a URL da API
Se a API rodar em outra porta, edite a primeira linha de `js/api.js`:

```js
const API_BASE = 'http://localhost:5000';
```

---

## Funcionalidades

| Funcionalidade | Descrição |
|---|---|
| **Login / Registro** | Autenticação JWT completa |
| **Feed — Todos os Posts** | Lista todos os posts da rede |
| **Feed — Posts dos Amigos** | Filtra apenas posts de amigos |
| **Criar Post** | Publica um novo post |
| **Curtir / Descurtir** | Toggle de curtida em posts |
| **Comentários** | Modal para ver e adicionar comentários |
| **Buscar Pessoas** | Busca usuários por nome |
| **Enviar Solicitação** | Adiciona amigos pela busca |
| **Solicitações Recebidas** | Aceitar ou recusar pedidos |
| **Solicitações Enviadas** | Cancelar pedidos enviados |
| **Lista de Amigos** | Ver e remover amigos |
| **Notificações** | Badge + painel de solicitações pendentes |

---

## Estrutura de arquivos

```
frontend/
├── index.html          # HTML principal (SPA)
├── css/
│   └── style.css       # Estilos (tema Facebook)
├── js/
│   ├── api.js          # Camada de comunicação com a API
│   ├── auth.js         # Login e registro
│   ├── feed.js         # Feed, posts, curtidas, comentários
│   ├── amigos.js       # Amizades e solicitações
│   ├── buscar.js       # Busca de usuários
│   └── app.js          # Inicialização, navegação e utilitários
└── README.md
```

## Tecnologias

- HTML5 / CSS3 / JavaScript puro (sem frameworks)
- Font Awesome 6 (ícones via CDN)
- JWT armazenado em `localStorage`
