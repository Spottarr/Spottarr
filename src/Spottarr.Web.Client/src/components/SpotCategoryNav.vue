<script setup lang="ts">
import SpotCategoryNavItem from "@/components/SpotCategoryNavItem.vue";
</script>

<template>
  <div class="space-y-2">
    <ul>
      <SpotCategoryNavItem title="Movies" color="sky" icon="movies">
        <li>DivX</li>
        <li>WMV</li>
        <li>MPG</li>
        <li>DVD5</li>
        <li>HD other</li>
        <li>Blu-ray</li>
      </SpotCategoryNavItem>
      <SpotCategoryNavItem title="Series" color="emerald" icon="movies">
        <li>DivX</li>
        <li>WMV</li>
        <li>MPG</li>
        <li>DVD5</li>
        <li>HD other</li>
        <li>Blu-ray</li>
      </SpotCategoryNavItem>
      <SpotCategoryNavItem title="Audio" color="amber" icon="movies">
        <li>Album</li>
        <li>Live set</li>
        <li>Podcast</li>
        <li>Audiobook</li>
      </SpotCategoryNavItem>
      <SpotCategoryNavItem title="Games" color="red" icon="movies">
        <li>Windows</li>
        <li>macOS</li>
        <li>Android</li>
        <li>Linux</li>
      </SpotCategoryNavItem>
      <SpotCategoryNavItem title="Applications" color="fuchsia" icon="movies">
        <li>Windows</li>
        <li>macOS</li>
        <li>Android</li>
        <li>Linux</li>
      </SpotCategoryNavItem>
    </ul>
  </div>
</template>
