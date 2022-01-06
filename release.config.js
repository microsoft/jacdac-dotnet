module.exports = {
    verifyConditions: [
        // Verifies a NUGET_TOKEN environment variable has been provided
        () => {
            if (!process.env.NUGET_TOKEN) {
                throw new SemanticReleaseErrorc(
                    'The environment variable NUGET_TOKEN is required.',
                    'ENOAPMTOKEN',
                )
            }
        },
        // Verifies the conditions for the plugins used below
        // For example verifying a GITHUB_TOKEN environment variable has been provided
        '@semantic-release/git',
        '@semantic-release/github',
    ],
    publish: [
        // https://github.com/semantic-release/git
        // Git plugin is need so the changelog file will be committed to the Git repository and available on subsequent builds in order to be updated.
        '@semantic-release/git',
    ],
}