# SteelWall — arquitetura e critérios de manutenção

## Estado observado

- Unity Editor: 6000.4.2f1.
- A lógica própria está concentrada em `Assets/Scripts` e ainda compila, em sua maior parte, na assembly predefinida `Assembly-CSharp`.
- O combate usa duas formações posicionais de quatro slots. A posição é um índice de formação, não o índice de uma lista compactada.
- `SkillSO`, `SkillEffectSO`, `SkillModifiersSO` e `StatusEffectSO` são definições persistentes; o estado mutável de uma execução deve pertencer à instância da execução.
- A interface observa eventos dos personagens e precisa tolerar objetos destruídos e referências ausentes.

## Direção arquitetural

1. **Domínio de combate:** regras de turno, iniciativa, estado da batalha, formação e morte.
2. **Personagens:** atributos-base, modificadores e estado dos efeitos; não devem conhecer detalhes de apresentação.
3. **Habilidades:** definições imutáveis em ScriptableObjects e estado temporário em `SkillData`.
4. **IA:** consome contexto de batalha já inicializado; não avalia alvos antes de receber a formação.
5. **Apresentação:** HUD, barras e indicadores reagem a eventos e tratam referências nulas/destruição.
6. **Infraestrutura:** entrada, carregamento de cenas e recursos devem ser pontos de integração explícitos, evitando dependências globais desnecessárias.

## Regras para futuras mudanças

- Não mover cenas, prefabs, ScriptableObjects ou scripts sem mover também os respectivos arquivos `.meta`. Os GUIDs fazem parte da identidade dos assets Unity.
- Não alterar nomes/assinaturas de campos serializados sem plano de migração e inspeção de cenas, prefabs e assets.
- Preservar slots vazios em listas posicionais; não compactar com `Where(x != null)` quando o índice representa uma posição.
- Tratar ScriptableObjects de definição como dados compartilhados. Não mutar assets de definição para armazenar estado de uma ação individual.
- Toda instância temporária de UnityEngine.Object criada por código precisa de proprietário e ciclo de vida explícitos.
- Corrotinas que fazem parte de uma máquina de estados devem ser aguardadas; eventos devem ser desinscritos com o mesmo delegate.
- Não introduzir singletons, event channels ou camadas de abstração sem um caso de uso concreto.

## Assemblies e testes — próxima etapa

Assembly Definitions são recomendáveis quando os limites reais de dependência estiverem claros. Antes de adicioná-las, mapear referências a Input System, Text Mesh Pro, UGUI e pacotes externos; criar assemblies por domínio apenas se isso reduzir dependências. Fazer a migração em etapas, validando a compilação após cada assembly.

Criar testes EditMode para cálculos puros (resistências, modificadores, seleção de alvos, limites de movimento e iniciativa) e testes PlayMode para ciclo de vida de combate, morte, HUD e corrotinas. Executar os testes no mesmo Editor Unity do projeto.

## Referências consultadas

- Unity — Assembly Definitions: https://docs.unity.com/en-us/engine/6000.5/manual/scripting/compilation-and-code-reload/script-compilation/assembly-definition-files/assembly-definitions-intro
- Unity — Serialization best practices: https://docs.unity.com/en-us/engine/6000.5/manual/scripting/compilation-and-code-reload/script-serialization/best-practices
- Unity — Organizing your project: https://activation.unity3d.com/how-to/organizing-your-project
- Unity — ScriptableObjects and modular architecture: https://unity.com/how-to/get-started-with-scriptableobjects-demo
- Reddit r/Unity3D — Organização de projetos Unity grandes e separação entre conteúdo próprio e assets de terceiros: https://www.reddit.com/r/Unity3D/comments/1vp9iib/how_do_you_organize_large_unity_projects/
- Reddit r/unity — Discussão sobre feature folders, assemblies e estrutura por domínio: https://www.reddit.com/r/unity/comments/1qn4j8w/the_folder_structure_mistake_that_kills_unity/
- Reddit r/Unity3D — Organização de sistemas reutilizáveis, ScriptableObjects e pacotes: https://www.reddit.com/r/Unity3D/comments/1q48kv6/best_way_to_organize_systems/

Discussões comunitárias são experiências práticas, não especificações. A documentação oficial da Unity é a referência para serialização e compilação.
