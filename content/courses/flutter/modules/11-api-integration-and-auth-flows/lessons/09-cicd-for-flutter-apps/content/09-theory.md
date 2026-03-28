---
type: "THEORY"
title: "Section 7: Monitoring and Notifications"
---

Setting up notifications keeps your team informed about build status and deployment results. Timely alerts enable quick responses to failures and celebrate successful releases.

### Slack Notifications

Integrate Slack to post build updates directly to your development channels:

```yaml
# codemagic.yaml
workflows:
  flutter-app:
    name: Flutter App CI/CD
    notifications:
      slack:
        channel: "#build-notifications"
        notify_on_success: true
        notify_on_failure: true
```

For GitHub Actions, use the Slack GitHub Action:

```yaml
- name: Notify Slack on Success
  if: success()
  uses: slackapi/slack-github-action@v1
  with:
    payload: |
      {
        "text": "✅ Flutter build succeeded for ${{ github.ref }}"
      }
  env:
    SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK }}
```

### Email Notifications (Codemagic)

Configure email alerts for stakeholders who need build visibility:

```yaml
publishing:
  email:
    recipients:
      - dev-team@company.com
      - qa-team@company.com
    notify:
      success: true
      failure: true
```

Email notifications work well for formal release communications and distribution to non-technical stakeholders. Combine multiple notification channels to ensure the right people receive updates through their preferred communication method.
