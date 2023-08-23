import { BlockArgDef, BlockDef } from "./blockly-types";

import blocks from "../data/blocks.json";
import extraBlocks from "../data/extra-blocks.json";
import prefabReferences from "../data/prefab-references.json";
import audioSourceReferences from "../data/audio-source-references.json";
import spriteReferences from "../data/sprite-references.json";
import towers from "../data/towers.json";
import upgrades from "../data/upgrades.json";
import { rowsList } from "./blockly-utils";
import { cloneDeep } from "lodash";

const createResourceBlock =
  (type: string) =>
  ([guid, name]: [string, string]) =>
    ({
      type: `${type}_${guid}`,
      output: type,
      category: "Other",
      subcategory: type.substring(type.lastIndexOf(".") + 1) + "s",
      colour: "243",
      message0: `${name}%1%2`,
      args0: [
        {
          type: "field_hidden",
          name: "$type",
          value: `${type.replace("Il2Cpp", "")}, Assembly-CSharp`,
        },
        {
          type: "field_hidden",
          name: "guidRef",
          value: guid,
        },
      ],
      comment: guid,
    } as BlockDef);

export const prefabReferenceMap = prefabReferences as Record<string, string>;
export const prefabBlocks = Object.entries(prefabReferences).map(
  createResourceBlock("Il2CppAssets.Scripts.Utils.PrefabReference")
);

export const audioSourceReferenceMap = prefabReferences as Record<
  string,
  string
>;
export const audioSourceBlocks = Object.entries(audioSourceReferences).map(
  createResourceBlock("Il2CppAssets.Scripts.Utils.AudioSourceReference")
);

export const spriteReferenceMap = spriteReferences as Record<string, string>;
export const spriteBlocks = Object.entries(spriteReferences).map(
  createResourceBlock("Il2CppAssets.Scripts.Utils.SpriteReference")
);

export const towerSetColors = {
  Primary: "200",
  Military: "108",
  Magic: "262",
  Support: "34",
  Hero: "53",
  None: "#888888",
};

export const towerIds = towers as Record<string, string>;
export const towerBlocks = Object.entries(towerIds).map(
  ([id, towerSet]) =>
    ({
      type: `Tower_${id}`,
      output: ["Tower", "string", towerSet],
      category: "Towers",
      subcategory: towerSet,
      colour: towerSetColors[towerSet] ?? undefined,
      message0: id,
      args0: [],
      data: id,
    } as BlockDef)
);

export const upgradeIds = upgrades as Record<string, string>;
export const upgradeBlocks = Object.entries(upgradeIds).map(
  ([id, name]) =>
    ({
      type: `Upgrade_${id}`,
      output: ["Upgrade", "string"],
      category: "Upgrades",
      colour: "#888888",
      message0: name,
      args0: [],
      data: id,
    } as BlockDef)
);

const towerModel = blocks.find(
  (block) => block.type === "Il2CppAssets.Scripts.Models.Towers.TowerModel"
) as BlockDef;

const customTowerModel = cloneDeep(towerModel) as BlockDef;

const remove = ["baseId", "tiers", "tier", "cost", "isSubTower", "mods"];

customTowerModel.type = "CustomTowerModel";
delete customTowerModel.category;
delete customTowerModel.extensions;

customTowerModel.message0 = "%1TowerModel";
customTowerModel.args0 = [customTowerModel.args0[0]];

for (let [i, name] of Object.entries(
  customTowerModel.mutatorOptions.optional
)) {
  if (remove.includes(name as string)) {
    customTowerModel[`message${i}`] = " ";
    customTowerModel[`args${i}`] = [];
  }
}

export const allJsonBlocks = [
  ...blocks,
  ...extraBlocks,
  ...prefabBlocks,
  ...audioSourceBlocks,
  ...spriteBlocks,
  ...towerBlocks,
  ...upgradeBlocks,
  customTowerModel,
] as BlockDef[];
