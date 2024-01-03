# Conventional Changelog

[![Conventional Commits][ccommits-img]][ccommits-url]

Conventional Changelog is a CLI-tool to generate a changelog from [conventional commits][ccommits-url].

Main features:

- Changelog generation from [conventional commits][ccommits-url]
- Changelog fine tuning using commit relationships
- Changelog corrections by overriding commit messages
- Deep customization of keywords (coming soon)

## Getting Started

Install conventional changelog using

```shell
dotnet tool install --global ConventionalChangelog
changelog "/path/to/repository" --output "changelog.md"
```

[ccommits-url]: https://conventionalcommits.org/
[ccommits-img]: https://badgen.net/badge/conventional%20commits/v1.0.0/dfb317

## Inspiration

This tool was inspired by

## Acknowledgements

Thank you to

- [Cocona](https://github.com/mayuki/Cocona#installing) for providing a great CLI framework
