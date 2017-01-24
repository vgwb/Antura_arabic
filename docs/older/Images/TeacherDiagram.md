graph TB
T(Teacher)
DB[Database] --> T
B[PlayerData] --> T
AR[ArabicHelper] --- T

T --> M[MiniGames]

M --> LOG
LOG[Log Manager] --> DB
