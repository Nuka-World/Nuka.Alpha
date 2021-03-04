FROM fluentd:v1.9.1-debian-1.0

USER root
ENV FLUENT_PLUGIN=fluentd/etc/plugin
RUN fluent-gem install fluent-plugin-elasticsearch
