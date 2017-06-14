# Minigame Egg

## Testing procedure
Total tests: 4
- Variations
	1. Egg_letters
    2. Egg_sequence
- Difficulty Levels: ininfluent


### Shortcuts
_none_

## Variations

### 1. Egg_letters
Player must find the correct letter.

#### Configuration Notes

#### Difficulty
- Higher difficulty hides visual hints
- The number of letters shown increases with difficulty

#### Scoring
- 3 stars if...
- 2 stars if...
- 1 star if...
---
### 2. Egg_sequence
Player must find the correct letters in order.

#### Configuration Notes

#### Difficulty
- Higher difficulty hides visual hints
- The number of letters shown increases with difficulty

#### Scoring
- 3 stars if...
- 2 stars if...
- 1 star if...
---
## Warnings to be fixed

**tons of**:
- [Warning] [BalloonsGame] [OnPoppedNonRequiredBalloon] Animator has not been initialized.
- [MakeWordPromptGreen] Animator has not been initialized.
- [Warning] [LetterPromptController] [OnStateChanged] Animator

## Optimization

there are many deactivate GameObjects.
some are very big, like `Environment_old` can they be deleted?
