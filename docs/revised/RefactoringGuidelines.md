2017 Refactoring Guidelines - EA4S - Antura and the Letters
=================

*Edits:*

<table>
  <tr>
    <td>11-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>


These notes attempt to provide notes and guidelines for refactoring the Antura code.

### Code annotations

The code is annotated to highlight pieces of code that should be refactored.
The following tags may be found throughout the code:

  * (refactor): the code should be logically refactored 
  * (obsolete): the code should be removed
  * (convention): the code should be refactored to match our conventions

### Documentation notes

The specific systems documentation files contain more general notes on refactoring partaining to that subsystem.
See the .md files for more information.

### General refactoring notes

   * Managers are scattered among many different folders and use widely different conventions. They should be refactored to show a common intention.