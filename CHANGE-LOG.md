# Change Log for Raygun.Aspire.Hosting.Raygun

### v2.0.0
- Added AI Error Resolution - Get AI suggestions to resolve exceptions via a locally running LLM in an Ollama container. This is an optional opt-in feature.
- Fixed a bug where exception messages containing characters that can't be used in file names wouldn't persist.

## The following initial versions used to live in [Raygun4Aspire](https://github.com/MindscapeHQ/raygun4aspire)

### v1.0.1
- Fixed a bug where on some machines the crash reports will fail to be saved locally due to the invalid "|" character being used in the file name. This character has now been replaced with "-". You may want to manually rename previously persisted crash reports as such in the "raygun-data" Docker volume.
- Fixed a bug where the date times would not be displayed in the locally running Raygun app depending on your local date time formatting options.

### v1.0.0
- Initial release - up to par with the Raygun4Net.AspNetCore package.
- When running in the local development environment, Raygun crash reports are sent to a locally running Raygun app that's fetched from [Docker Hub](https://hub.docker.com/r/raygunowner/raygun-aspire-portal).
- Optionally providing a Raygun application API Key allows crash reports to be sent to your Raygun cloud service account when your Aspire app is running in production.