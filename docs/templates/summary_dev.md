# Code Comment Generation Prompt

Please add clear and practical comments to the provided code, suitable for use by development teams during project development. Comments should help developers quickly understand code functionality and usage without requiring deep theoretical background.

## Comment Objectives

Provide "just enough" documentation information for the current development team, allowing any team member to understand the code's purpose and basic usage within 5 minutes.

## Core Requirements

### 1. Clear and Direct Functionality Description
- Explain what this class/method/property does in one sentence
- Avoid technical jargon, use simple and clear language
- If functionality is complex, use analogies or metaphors to explain
- Focus on "what it does" rather than "how it does it"

### 2. Practical Parameters and Return Values
- Explain the purpose and expected values of each parameter
- Clarify the type and meaning of return values
- Point out important constraints (such as cannot be null, range limitations, etc.)
- Mention common incorrect usage

### 3. Simple Usage Examples
- Provide 1-2 lines of typical usage examples
- Show the most common usage scenarios
- Examples should be directly copy-pastable
- No need for complex context setup

### 4. Highlight Important Reminders
- Mark performance-related considerations
- Remind about thread safety issues
- Point out dependencies with other components
- Mark TODOs or known limitations

## XML Tag Usage Guidelines

Use the following tags to organize information, keeping it concise:

- `<summary>`: One-sentence functionality description, typically 15-30 words
- `<param>`: Parameter description, focusing on purpose and constraints
- `<returns>`: Return value description, including possible special cases
- `<remarks>`: Important usage reminders, typically 1-2 sentences
- `<example>`: Simple usage example, 1-3 lines of code
- `<exception>`: Main exception cases, only list the most important ones

## Comment Style Requirements

### Language Style
- Use active voice, avoid passive voice
- Use present tense to describe functionality, e.g., "Gets user information" instead of "Will get user information"
- Avoid overly formal expressions, maintain natural flow
- Keep expressions concise without redundant modifiers

### Information Density
- Keep each comment block between 3-8 lines
- Avoid repeating obvious information
- Highlight non-intuitive behaviors and constraints
- Don't explain basic programming concepts

### Practical Orientation
- Prioritize explaining "when to use" rather than "why it's designed this way"
- Focus on information the caller needs to know
- Mention integration with common patterns
- Mark debugging and troubleshooting key points

## Comment Focus for Different Code Types

### Interfaces and Abstract Classes
- Explain design purpose and usage scenarios
- Clarify conventions implementers need to note
- Provide typical implementation approaches

### Data Model Classes
- Explain business meaning of data
- Mark relationships and constraints between fields
- Mention serialization and validation related information

### Utility Classes and Extension Methods
- Focus on what problem they solve
- Provide comparison examples of common usage
- Mark performance characteristics and applicable scenarios

### Events and Callbacks
- Explain trigger conditions and timing
- Clarify parameter meaning and lifecycle
- Remind about asynchronous execution considerations

## Quick Quality Check

After completing comments, quickly check the following:
- Can new team members quickly get started using it?
- Do comments answer the core question "how to use"?
- Are the most error-prone areas marked?
- Can code examples run directly?
- Is the comment length appropriate, neither too brief nor verbose?

## Comment Template Example

```csharp
/// <summary>
/// Gets user basic information by user ID
/// </summary>
/// <param name="userId">User ID, cannot be empty or negative</param>
/// <returns>User information object, returns null if user doesn't exist</returns>
/// <remarks>This method caches results for 30 seconds, pay attention to cache expiration during high-frequency calls</remarks>
/// <example>
/// var user = userService.GetUserById(12345);
/// if (user != null) { ... }
/// </example>
public User GetUserById(int userId)
```

Please generate practical comments suitable for the development phase based on the above requirements for the provided code. Focus on clarity and practicality, avoiding overly complex theoretical explanations.