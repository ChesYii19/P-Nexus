# Protocol Nexus

**Jogo de Plataforma 2D desenvolvido em Unity.**
Este projeto foi desenvolvido como peça de portfólio e requisito acadêmico para a disciplina de Game Development, focando na aplicação de conhecimentos avançados de C#, engenharia de software e design de níveis.

## Descrição do Jogo

**Protocol Nexus** é um jogo de plataforma e ação 2D ambientado em um futuro cyberpunk caótico, focado no esgoto (Sewers) da megalópole *Neon District*. O jogador controla um soldado desonesto encarregado de invadir instalações da corporação Nexus. 

O jogo conta com **4 fases procedurais** com nível de dificuldade crescente. O design do jogo evita soluções genéricas de arrastar e soltar da Unity, focando fortemente em **geração processual de níveis** e **máquinas de estados de animação escritas do zero** via código.

## Mecânicas Implementadas

- **Movimentação Completa:** O personagem pode andar, correr, pular, atirar e interagir com escadas (escalar).
- **Inimigos com IA Básica:** Inimigos terrestres que patrulham plataformas e inimigos voadores.
- **Sistema de Fases Processual:** As 4 fases são geradas algoritmicamente. Conforme a fase avança, o tamanho do nível, buracos, e a densidade de inimigos aumenta proporcionalmente.
- **Transição de Cenas:** Portais de transição entre os níveis gerenciados por um `GameManager` persistente (Singleton).
- **HUD Funcional:** Contagem de vidas, pontuação (moedas coletáveis) e exibição do nível atual.
- **Sistema de Áudio:** Efeitos sonoros ao pular, coletar itens e sofrer dano, atrelados a um AudioManager persistente.

## Controles

| Ação | Teclado |
| :--- | :--- |
| **Mover para a Esquerda** | Seta Esquerda ou `A` |
| **Mover para a Direita** | Seta Direita ou `D` |
| **Pular** | Barra de Espaço |
| **Escalar (Subir Escadas)** | Seta para Cima ou `W` |
| **Descer Escadas** | Seta para Baixo ou `S` |
| **Atirar** | `Z` ou `Botão Esquerdo do Mouse` |
| **Pausar Jogo** | `ESC` |

## Instruções de Execução

### Para Avaliadores / Professores (Versão Compilada)
1. Baixe o arquivo compactado contendo o Executável (.exe).
2. Extraia todos os arquivos em uma mesma pasta.
3. Clique duas vezes em `ProtocolNexus.exe`.
4. Use os controles acima para jogar.

### Para Desenvolvedores (Rodando pela Unity)
O projeto foi desenvolvido para que todo o seu conteúdo seja gerado de forma autônoma (Geração de Prefabs e Fases).
1. Clone este repositório do GitHub.
2. Abra o projeto na **Unity 6000.x** (ou superior compatível).
3. Na barra de menus superior, clique em:
   `Protocol Nexus -> 6 - GERAR O JOGO COMPLETO (FINAL)`
4. Aguarde a mensagem de sucesso no Console. O script fará o fatiamento (slice) automático das spritesheets, construirá os prefabs e gerará as cenas proceduralmente.
5. Abra a pasta `Assets/Scenes`, carregue a cena `Nivel1` e pressione o botão **Play**.

---

## Estrutura do Repositório (Conforme Exigência)

- `/Assets/Scripts/`: Códigos-fonte organizados (C#). Destaque para `LevelGenerator.cs` (geração de fases) e `SpriteAnimator.cs` (máquina de animação).
- `/Assets/Sprites/`: Sprites de cenários, inimigos e jogador.
- `/Assets/Audio/`: Trilha sonora e efeitos sonoros (SFX).
- `creditos_e_licencas.md`: Arquivo declaratório contendo os devidos créditos aos assets visuais retirados do Itch.io.
- `Prints/`: Capturas de tela (screenshots) das telas e níveis.
