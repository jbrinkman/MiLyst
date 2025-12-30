---
auto_execution_mode: 1
description: Review and address each comment on a pull request.
---

**Steps**

1. Check out the PR branch

2. Get the latest changes from this branch in the remote repository

3. Get comments on PR

4. For EACH comment, do the following. Remember to address one comment at a time.
 a. Print out the following: "(index). From [user] on [file]:[lines] — [body]"
 b. Analyze the file and the line range.
 c. If you don't understand the comment, do not make a change. Just ask me for clarification, or to implement it myself.
 d. If you think the change is unnecessary, do not make a change. Provide me your reasoning on why we should not implement the change. Then ask me whether to make the change or to skip the change.
 d. If you think you can make the change, and you agree that the change is necessary, make the change BEFORE moving onto the next comment.

5. After all comments are processed, summarize what you did, and which comments need the USER's attention.