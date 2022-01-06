module.exports = {
    "branches": ["main"],
    "plugins": [
        '@semantic-release/commit-analyzer',
        '@semantic-release/release-notes-generator',
        '@semantic-release/github',
        ["@semantic-release/exec", {
            "publishCmd": "./dotnet nuget publish"
        }],
    ],
}