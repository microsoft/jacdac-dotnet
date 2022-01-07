module.exports = {
    "branches": ["main"],
    "plugins": [
        '@semantic-release/commit-analyzer',
        '@semantic-release/release-notes-generator',
        '@semantic-release/github',
        [
            "semantic-release-dotnet",
            {
                packArguments: ["Jacdac.sln", "--nologo", "-c", "Release"],
                pushFiles: "bin/*.nupkg",
            },
        ],
    ],
}