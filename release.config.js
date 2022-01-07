module.exports = {
    "branches": ["main"],
    "plugins": [
        '@semantic-release/commit-analyzer',
        '@semantic-release/release-notes-generator',
        '@semantic-release/github',
        [
            "semantic-release-dotnet",
            {
                paths: ["./Directory.Build.csproj"],
            }
        ],
        [
            "semantic-release/exec",
            {
                "publishCmd": "dotnet nuget push packages/*.nupkg -k $NUGET_TOKEN -s $NUGET_PUSH_URL",
            }
        ]
    ],
}