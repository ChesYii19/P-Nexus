# Créditos e Licenças (Copyright e Atribuições)

Para fins de avaliação acadêmica e transparência de desenvolvimento, declaro abaixo a origem de todos os assets gráficos e sonoros utilizados no desenvolvimento de **Protocol Nexus**. Este projeto é de cunho puramente educacional e não comercial.

## Assets Visuais (Sprites e Pixel Art)

Todos os assets visuais foram obtidos através de repositórios gratuitos focados em desenvolvedores independentes na plataforma **Itch.io**. As URLs diretas para os pacotes utilizados (conforme histórico de aquisição) são:

1. **Cenário e Player (Tileset, Backgrounds e Soldier):**
   - **Pacote Base:** Sewers Action Pack
   - **Criador:** Ansimuz
   - **URL:** [https://ansimuz.itch.io/sewers-action-pack](https://ansimuz.itch.io/sewers-action-pack)
   - **Uso:** Construção do nível procedural (chão de esgoto e parallax) e frames de animação do personagem principal.

2. **Inimigos (Patrulha, Atirador Voador e Boss):**
   - **Pacote Base:** Free City Enemies Pixel Art Sprite Sheets
   - **Criador:** Free Game Assets
   - **URL:** [https://free-game-assets.itch.io/free-city-enemies-pixel-art-sprite-sheets](https://free-game-assets.itch.io/free-city-enemies-pixel-art-sprite-sheets)
   - **Uso:** Implementação dos inimigos utilizando a máquina de estados personalizada (`SpriteAnimator.cs`).

3. **Efeitos Especiais (Tiros e Impactos):**
   - **Pacote Base:** Warped Shooting FX
   - **Criador:** Ansimuz
   - **URL:** [https://ansimuz.itch.io/warped-shooting-fx](https://ansimuz.itch.io/warped-shooting-fx)
   - **Uso:** Efeitos de tiro da arma do jogador e impactos de colisão.

4. **Assets Alternativos (Em testes / Versões Anteriores):**
   - *Neon District Cyberpunk Kit* (abk911lol): [https://abk911lol.itch.io/neon-district-free-cyberpunk-pixel-art-starter-kit-16x16-unitygodotrpg-maker](https://abk911lol.itch.io/neon-district-free-cyberpunk-pixel-art-starter-kit-16x16-unitygodotrpg-maker)
   - *Robot Dog e Spider Drone* (Vivicat): [https://vivicat.itch.io/robot-dog-sprite-sheet](https://vivicat.itch.io/robot-dog-sprite-sheet) / [https://vivicat.itch.io/spider-drone](https://vivicat.itch.io/spider-drone)

## Assets de Áudio

1. **Efeitos Sonoros (SFX):**
   - **Fonte:** Bibliotecas de áudio gratuitas (como Freesound.org) e pacotes complementares da Unity Asset Store. (Sons de pulo, dano, coleta de moedas e alerta de chefe).

2. **Trilha Sonora (BGM):**
   - **Uso:** Músicas de ambientação Cyberpunk.
   - **Licença:** Royalty-Free (Uso não-comercial liberado para projetos estudantis).

## Código-Fonte

Toda a arquitetura lógica em C#, incluindo a **Geração Procedural de Fases**, a **Máquina de Estados de Animação Customizada** e o **Sistema de Fábrica de Prefabs**, foi escrita 100% do zero para este trabalho acadêmico. 

---
*Este documento garante que o projeto respeita as diretrizes de direitos autorais e licenças (maioria sob CC0 ou licença padrão itch.io para uso livre), deixando claro o que foi programado (toda a lógica C#) e o que foi adquirido de repositórios gratuitos (arte visual e som), alinhando-se aos rigorosos requisitos da disciplina.*
