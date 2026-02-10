# Harry Workflow
A structured, repeatable process for reviewing Harry’s recommendations, logging outcomes, issuing lessons learned, and reinforcing doctrine.

---

# 1. Proposal Phase
Harry generates a recommendation and outputs:
- Code-only
- No commentary
- No creativity
- No modifications to existing files
- Only new stubs or patches
- Saved under: `/Tools/Harry/Changes/`

Each proposal receives a unique ID:
`Change_XXXX`

---

# 2. Review Phase (Human)
The human reviewer:
1. Reads the proposal
2. Evaluates correctness
3. Checks for rule compliance
4. Identifies violations or drift
5. Decides whether to accept, reject, or modify

Reviewer creates:
- A Training Log entry
- (Optional) A Lessons Learned entry

---

# 3. Logging Phase (PowerShell)
PowerShell appends a new entry to:

`/Tools/Harry/Harry_TrainingLog.md`

The entry includes:
- Proposal ID
- Correct behaviors
- Incorrect behaviors
- Rules followed
- Rules violated
- Reviewer notes
- Lessons Learned ID (if any)
- Harry’s next output ID (once received)

This creates a chronological, auditable history.

---

# 4. Lessons Learned Phase (If Needed)
If drift or violations occur:
- Reviewer writes a Lessons Learned entry
- PowerShell appends it to:

`/Tools/Harry/Harry_LessonsLearned.md`

Each lesson includes:
- What went wrong
- What went right
- Doctrine reinforcement
- Required behavior going forward

This becomes the feedback Harry receives.

---

# 5. Feedback Phase (Human → Harry)
Reviewer sends Harry:
- The Lessons Learned entry
- The Training Log summary (optional)
- Any clarifications or expectations

Harry uses this to adjust his next proposal.

---

# 6. Adaptation Phase (Harry)
Harry produces:
- A new proposal
- With a new ID
- Ideally showing improvement
- Logged again through the same cycle

This creates a closed-loop training system.

---

# 7. Continuous Improvement
Over time, the system builds:
- A full behavioral history
- A record of drift and correction
- A measurable improvement curve
- A stable doctrine Harry internalizes

This ensures Harry remains a helper, not an autonomous modifier.

---

# End of Workflow