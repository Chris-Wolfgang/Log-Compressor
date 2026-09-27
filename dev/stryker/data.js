window.BENCHMARK_DATA = {
  "lastUpdate": 1790490300910,
  "repoUrl": "https://github.com/Chris-Wolfgang/Log-Compressor",
  "entries": {
    "Mutation score": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "5f79f5654354fd750f3a2e2c1b303cde478af375",
          "message": "release: v0.4.1 (#301)\n\nPATCH release. Since v0.4.0 the shipped binary changes only by the 10.0.12\nservicing bump of four Microsoft runtime packages (Hosting,\nFileSystemGlobbing, ConfigurationManager, IO.Hashing) and three S6667\nsuppressions with no behaviour change (#283). Everything else in the 42\ncommits is CI, tests and docs: template workflow sync, the protected-files\nguard, Scorecard publishing (#297), the Stryker PR gate and floor 91\n(#298/#300), 100 % test-assembly coverage, the fuzz coverage flake (#299).\n\nCHANGELOG assembled with scripts/changelog.ps1 from 2 fragments (one added\nhere for the Dependabot bump, which carries no fragment of its own);\ncompare links updated. Released from main: vNext holds only #220, which\n#283/#284 superseded.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-26T22:27:29Z",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/5f79f5654354fd750f3a2e2c1b303cde478af375"
        },
        "date": 1790490299008,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 92.52,
            "unit": "%"
          }
        ]
      }
    ]
  }
}