#!/usr/bin/env python3
"""Re-enable the main-branch ruleset after an admin-bypass release cycle.

Run this yourself — the agent's harness blocks ruleset PUT mutations as a
security-settings change. Usage:  python scripts/Enable-BranchRuleset.py
"""
import json
import subprocess
import sys

OWNER_REPO = "Chris-Wolfgang/Log-Compressor"
RULESET_ID = 15684702


def main() -> int:
    # Capture stdout only (the JSON); gh's stderr (auth prompts, HTTP errors)
    # passes through so a failure shows the actionable message before this
    # script exits with gh's status code.
    try:
        full = json.loads(subprocess.run(
            ["gh", "api", f"repos/{OWNER_REPO}/rulesets/{RULESET_ID}"],
            stdout=subprocess.PIPE, text=True, check=True).stdout)

        # A naive PUT is rejected over allowed_dismissal_actors on
        # pull_request rules.
        for rule in full.get("rules") or []:
            if rule.get("type") == "pull_request" and isinstance(rule.get("parameters"), dict):
                rule["parameters"].pop("allowed_dismissal_actors", None)

        body = {
            "name": full["name"],
            "target": full["target"],
            "enforcement": "active",
            "bypass_actors": full.get("bypass_actors") or [],
            "conditions": full.get("conditions") or {},
            "rules": full.get("rules") or [],
        }
        subprocess.run(
            ["gh", "api", "-X", "PUT", f"repos/{OWNER_REPO}/rulesets/{RULESET_ID}", "--input", "-"],
            input=json.dumps(body), text=True, check=True)
    except subprocess.CalledProcessError as error:
        return error.returncode

    print(f"Re-enabled ruleset {RULESET_ID} ({full['name']}).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
