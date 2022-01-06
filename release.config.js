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
    ],
    publish: [
    ],
}