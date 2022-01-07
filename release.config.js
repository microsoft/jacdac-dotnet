module.exports = {
    "branches": ["main"],
    "plugins": [
        '@semantic-release/commit-analyzer',
        '@semantic-release/release-notes-generator',
        '@semantic-release/github',
        [
            "semantic-release-dotnet",
            {
                paths: ["./Directory.Build.props"],
                debug: true
            }
        ],
        [
            "@semantic-release/exec",
            {
                "publishCmd": "sh ./nuget.sh",
            }
        ]
    ],
}