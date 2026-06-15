# Relatório Teórico de Desenvolvimento: Protocol Nexus

**Disciplina:** Game Development
**Data:** Junho de 2026
**Autor:** [Seu Nome/Grupo]

---

## 1. Introdução
Este relatório detalha as escolhas técnicas, mecânicas implementadas e os desafios superados no desenvolvimento do jogo "Protocol Nexus". O projeto consiste em um jogo de plataforma 2D desenvolvido no motor gráfico Unity (versão 2022.3), com o intuito de aplicar na prática os fundamentos de programação de scripts, simulação física, design de níveis e inteligência artificial básica.

## 2. Implementação das Mecânicas Principais

### 2.1 Sistema de Movimentação (Física vs Input)
A base do movimento do protagonista Tatsuya Yuki foi construída separando estritamente a coleta de entradas (Inputs) da aplicação de forças físicas. 
- **Input:** Utilizou-se o método `Update()` para capturar o `Input.GetAxisRaw("Horizontal")`, garantindo resposta imediata sem o amortecimento indesejado de eixos suavizados.
- **Física:** A aplicação na velocidade do `Rigidbody2D` ocorre exclusivamente no `FixedUpdate()`, alinhado com o ciclo de física da Unity. A corrida foi implementada como um modificador de velocidade acionado pelo `LeftShift`.

### 2.2 Sistema de Pulo e Detecção de Chão
O pulo não depende do motor de física nativo para detecção de colisão contínua. Em vez disso, foi criado um sistema de `OverlapCircle` (Raycasting em área) atrelado aos pés do personagem. Esse círculo verifica se há contato com a camada "Chao". Isso eliminou o bug do "Pulo Duplo Infinito" e evitou que o personagem grudasse nas paredes.

### 2.3 Mecânicas de Escalada
Ao colidir com a *tag* "Escada", a gravidade (`gravityScale`) do personagem é anulada e o input vertical passa a dominar a velocidade no eixo Y. Ao sair do *Trigger*, a gravidade padrão é restaurada instantaneamente.

## 3. Inteligência Artificial Básica

Para dar vida ao cenário, implementamos dois tipos de comportamento autônomo para os inimigos (Cães e Aranhas Voadoras):
- **Patrulha Terrestre:** Os cães utilizam uma combinação de `Raycast2D` disparados para frente e para baixo. O *raycast* inferior detecta precipícios (bordas de plataformas) e o *raycast* frontal detecta obstáculos. Quando qualquer um desses gatilhos é acionado, a IA inverte a escala X do *Sprite* e seu vetor de movimento, criando um loop de patrulha perfeito que não requer "paredes invisíveis" para limitá-los.
- **Sistema de Dano:** Foi utilizado o sistema de *Triggers* do `Collider2D` e `CompareTag` (ou `.name.Contains`) para registrar interações agressivas, ativando rotinas de invulnerabilidade temporária (I-Frames) controladas por *Coroutines* para não punir o jogador injustamente em múltiplos *frames*.

## 4. Estrutura do Level Design e Câmera

O Level Design baseia-se em *Prefabs* modulares. Criou-se blocos padronizados de 5x2 unidades que podem ser alinhados para formar longos corredores e plataformas precisas para pulos calculados.

Para evitar desorientação visual:
- A câmera não é estática. Foi acoplado um script de suavização matemática (`Vector3.Lerp`) que segue o jogador suavemente.
- O sistema de restrição de tela (Bounds) foi ajustado para permitir ampla visão horizontal, compensando as proporções de uma cena puramente 2D sem fundo complexo, impedindo que o jogador se sinta em "tela cega".

## 5. Resolução de Desafios Técnicos e Bugs

Durante as fases de teste, lidamos com problemas clássicos de manipulação de *Sprite Sheets* e Controladores de Animação (Animator). 
A ausência inicial de um `RuntimeAnimatorController` gerou "warnings" que, somados ao bloqueio prematuro do eixo da câmera, ocultavam a movimentação do jogador. Solucionamos o problema injetando dinamicamente a referência do Sprite via `AssetDatabase` em tempo de execução no script de controle principal, garantindo estabilidade e visualização correta do personagem desde o *Frame 1*.

## 6. Conclusão
"Protocol Nexus" consolida com êxito os conhecimentos da disciplina. A modularidade do código permite que as 4 fases construídas sejam facilmente expandidas em um projeto maior, além de provar a solidez dos conceitos de Componentização e Game Loop da Unity.
