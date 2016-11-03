# Teacher

this module deals with all data, the course, the player profile and feeds the minigames

```{mermaid}
graph TB
T(Teacher)
DB[Database] --> T
B[PlayerData] --> T
AR[ArabicHelper] --- T

T --> M[MiniGames]

M --> LOG
LOG[Log Manager] --> DB
```
