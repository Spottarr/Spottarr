<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router';
import SideBar from "@/components/SideBar.vue";
</script>

<template>
  <div class="flex">
    <aside class="w-72 flex-none relative h-screen p-4 space-y-4 overflow-y-auto shadow bg-white dark:bg-slate-800">
      <SideBar />
    </aside>

    <main class="w-full flex-1 overflow-x-hidden h-screen">
      <div class="p-4 overflow-y-scroll">
        <div class="p-4 w-full rounded-md shadow bg-white dark:bg-slate-800">
          <RouterView />
        </div>
      </div>
    </main>
  </div>
</template>
