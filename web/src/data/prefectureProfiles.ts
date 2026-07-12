export type PrefectureProfile = {
  name: string;
  region: string;
  focusVenues: string[];
};

export const prefectureProfiles: PrefectureProfile[] = [
  { name: "北海道", region: "北海道", focusVenues: ["サンコーボウル", "ラウンドワン札幌すすきの店", "ディノスボウル札幌手稲"] },
  { name: "青森", region: "東北", focusVenues: ["弘前ファミリーボウル", "ゆりの木ボウル"] },
  { name: "岩手", region: "東北", focusVenues: ["盛岡スターレーン", "ラウンドワン盛岡店"] },
  { name: "宮城", region: "東北", focusVenues: ["コロナキャットボウル仙台店", "ラウンドワン仙台苦竹店"] },
  { name: "秋田", region: "東北", focusVenues: ["ロックンボウル", "ラウンドワン秋田店"] },
  { name: "山形", region: "東北", focusVenues: ["山形ファミリーボウル", "ヤマコーボウル"] },
  { name: "福島", region: "東北", focusVenues: ["ボウルアピア郡山", "ラウンドワン福島店"] },
  { name: "茨城", region: "関東", focusVenues: ["大学ボウル土浦本店", "フジ取手ボウル"] },
  { name: "栃木", region: "関東", focusVenues: ["足利スターレーン", "宇都宮ゴールドレーン"] },
  { name: "群馬", region: "関東", focusVenues: ["桐生スターレーン", "パークレーン高崎"] },
  { name: "埼玉", region: "関東", focusVenues: ["川口スプリングレーン", "浦和スプリングレーン", "新狭山グランドボウル"] },
  { name: "千葉", region: "関東", focusVenues: ["アイキョーボウル", "本八幡スターレーン", "柏ヤングボウル"] },
  { name: "東京", region: "関東", focusVenues: ["高田馬場グランドボウル", "東京ポートボウル", "新宿コパボウル"] },
  { name: "神奈川", region: "関東", focusVenues: ["川崎グランドボウル", "相模原パークレーンズ", "スポルト八景"] },
  { name: "新潟", region: "甲信越・北陸", focusVenues: ["グランドボウル黒埼", "ラウンドワン新潟店"] },
  { name: "富山", region: "甲信越・北陸", focusVenues: ["富山地鉄ゴールデンボウル", "ノースランドボウル呉羽"] },
  { name: "石川", region: "甲信越・北陸", focusVenues: ["クァトロブーム鹿島", "コロナキャットボウル金沢店", "コロナキャットボウル小松店"] },
  { name: "福井", region: "甲信越・北陸", focusVenues: ["コロナキャットボウル福井店", "コロナキャットボウル福井春江店"] },
  { name: "山梨", region: "甲信越・北陸", focusVenues: ["ダイトースターレーン", "都留ファミリーボウル"] },
  { name: "長野", region: "甲信越・北陸", focusVenues: ["ヤングファラオ", "アピナボウル長野篠ノ井店"] },
  { name: "岐阜", region: "東海", focusVenues: ["コロナキャットボウル大垣店", "ACグランド"] },
  { name: "静岡", region: "東海", focusVenues: ["柿田川パークレーンズ", "狐ヶ崎ヤングランドボウル", "藤枝グランドボウル"] },
  { name: "愛知", region: "東海", focusVenues: ["小牧コロナキャットボウル", "稲沢グランドボウル", "イーグルボウル"] },
  { name: "三重", region: "東海", focusVenues: ["鈴鹿グランドボウル", "津グランドボウル"] },
  { name: "滋賀", region: "関西", focusVenues: ["愛知川ボウル", "栗東ボウリングジム", "ラピュタボウル彦根"] },
  { name: "京都", region: "関西", focusVenues: ["MKボウル上賀茂", "キョーイチボウル宇治", "しょうざんボウル"] },
  { name: "大阪", region: "関西", focusVenues: ["WAVE34", "サンスクエアボウル", "ラウンドワンスタジアム堺中央環状店"] },
  { name: "兵庫", region: "関西", focusVenues: ["神戸六甲ボウル", "ジェームス山グランドボウル", "アルゴボウル"] },
  { name: "奈良", region: "関西", focusVenues: ["トドロキボウル", "ラウンドワン奈良ミ・ナーラ店"] },
  { name: "和歌山", region: "関西", focusVenues: ["和歌山グランドボウル", "ラウンドワン和歌山店"] },
  { name: "鳥取", region: "中国", focusVenues: ["鳥取スターボウル", "YSPボウル"] },
  { name: "島根", region: "中国", focusVenues: ["出雲会館センターボウル", "松江センターボウル"] },
  { name: "岡山", region: "中国", focusVenues: ["岡山ヤングボウル", "コーシンボウル"] },
  { name: "広島", region: "中国", focusVenues: ["広島パークレーン", "コロナキャットボウル福山店"] },
  { name: "山口", region: "中国", focusVenues: ["くだまつボウル", "ユーズボウル萩"] },
  { name: "徳島", region: "四国", focusVenues: ["スエヒロボウル", "ラウンドワン徳島・万代店"] },
  { name: "香川", region: "四国", focusVenues: ["太洋ボウル", "MG-BOWL屋島"] },
  { name: "愛媛", region: "四国", focusVenues: ["キスケボウル", "ラウンドワン松山店"] },
  { name: "高知", region: "四国", focusVenues: ["ボウルかつらしま", "ラウンドワン高知店"] },
  { name: "福岡", region: "九州・沖縄", focusVenues: ["コロナキャットボウル小倉店", "折尾スターレーン"] },
  { name: "佐賀", region: "九州・沖縄", focusVenues: ["ボウルアーガス", "遊道楽嘉瀬店"] },
  { name: "長崎", region: "九州・沖縄", focusVenues: ["長崎ラッキーボウル", "諫早パークレーン"] },
  { name: "熊本", region: "九州・沖縄", focusVenues: ["スポラ九品寺", "パスカワールド宇土店"] },
  { name: "大分", region: "九州・沖縄", focusVenues: ["タワーボウル萩原", "OBSボウル"] },
  { name: "宮崎", region: "九州・沖縄", focusVenues: ["宮崎エースレーン", "ラウンドワン宮崎店"] },
  { name: "鹿児島", region: "九州・沖縄", focusVenues: ["T-MAX BOWL", "サンライトゾーン"] },
  { name: "沖縄", region: "九州・沖縄", focusVenues: ["サラダボウル", "ラウンドワンスタジアム沖縄・宜野湾店", "スカイレーン"] }
];

export const allPrefectureNames = prefectureProfiles.map((profile) => profile.name);
