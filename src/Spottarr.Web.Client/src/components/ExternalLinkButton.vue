<script setup lang="ts">
import type { DefineComponent } from 'vue';

defineProps<{ url: string; icon: DefineComponent }>();
</script>

<template>
  <a class="" target="_blank" :href="url">
    <button class="rounded-md dark:bg-slate-600 dark:hover:bg-slate-500 p-2">
      <component :is="icon" class="size-5 flex-none fill-slate-900 dark:fill-white" />
    </button>
  </a>
</template>

<style scoped></style>
