# Basic Reason Template

Hello! This project allows you to quickly get started with Reason and BuckleScript. If you wanted a more sophisticated version, try the `react` template (`bsb -theme react -init .`).

# Build
```
yarn build
```

# Build + Watch

```
yarn watch
```

# Run
Runs the stack-overflow Rest service
```
yarn serve
```

## API
`/tags/:tag` returns an array of tags
```
[
  {
    count: int,
    name: string
  }
]
```

# Test API
Postman works well here or if you want something simpler.
```
curl -i -H "Accept: application/json" -H "Content-Type: application/json" -X GET http://127.0.0.1:3000/tags/and
```

# Editor
If you use `vscode`, Press `Windows + Shift + B` it will build automatically
