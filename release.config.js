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
            "semantic-release-nuget",
            {
                packArguments: ["Jacdac.sln", "--nologo", "-c", "Release", "-o", "packages"],
                pushFiles: "packages/*.nupkg",
            },
        ],
    ],
}