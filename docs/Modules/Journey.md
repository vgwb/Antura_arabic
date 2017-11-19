# Journey

Represents the didactical journey.

## Journey position

The Journey position is defined as a hierarchical structure, made of Stages, Learning Blocks, and Play Sessions.

* **Stages** define overall learning goals. Each stage is assigned to a specific Map.
* **Learning Blocks** define general learning requirements for a set of play sessions.
* **Play Sessions** define single play instances, composed of several minigames and a result. A Play Session may be considered an **Assessment**, in this case the value is always 100.

Each is defined by a sequential integer value.
A combination of three values identifies a single playing session, which is referred to as **Journey Position**.

A Journey Position is thus identified by a the sequence **X.Y.Z** where X is the Stage, Y the Learning Block, and Z the Play Session.