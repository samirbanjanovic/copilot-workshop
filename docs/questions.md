# Questions

> #### Does GitHub Copilot include sensitive files when collecting contextual data?

It is my understanding it will include any file available for its context and suggest content in them. However, you can configure configure Copilot to exclude them using `Content Exclusion (Beta)` configuration as described [here](https://docs.github.com/en/copilot/managing-copilot-business/configuring-content-exclusions-for-github-copilot) and [here](https://github.blog/changelog/2023-11-08-copilot-content-exclusion-is-now-available-in-public-beta/).

> #### What is Copilot's token limit?

Based on [this source](https://github.blog/2023-05-17-how-github-copilot-is-getting-better-at-understanding-your-code/) the character limit is 6000 characters. A token makes up about one to two words, 

``